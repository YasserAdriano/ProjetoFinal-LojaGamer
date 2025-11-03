using LojaGamerApi.Data;
using LojaGamerApi.Dtos;
using LojaGamerApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LojaGamerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Define a URL base como /api/Auth
    public class AuthController : ControllerBase
    {
        private readonly LojaGamerContext _context;
        private readonly IConfiguration _configuration;

        // "Injeção de Dependência": Pede ao .NET para nos dar acesso ao Banco (Context) e às Configs (Configuration)
        public AuthController(LojaGamerContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // --- ENDPOINT DE REGISTRO ---
        // URL: POST /api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            // 1. Verifica se já existe um usuário com este e-mail
            if (await _context.Usuarios.AnyAsync(u => u.Email == registerDto.Email))
            {
                return BadRequest("Este e-mail já está em uso.");
            }

            // 2. "Embaralha" (hash) a senha usando o BCrypt
            string senhaHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Senha);

            // 3. Cria a nova entidade Usuario
            var usuario = new Usuario
            {
                Nome = registerDto.Nome,
                Email = registerDto.Email,
                SenhaHash = senhaHash,
                Role = "Cliente" // Todo novo registro é "Cliente" por padrão
            };

            // 4. Adiciona o usuário ao banco de dados
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync(); // Salva as mudanças

            return Ok(new { message = "Usuário registrado com sucesso!" });
        }

        // --- ENDPOINT DE LOGIN ---
        // URL: POST /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            // 1. Procura o usuário pelo e-mail
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            // 2. Verifica se o usuário existe E se a senha está correta
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(loginDto.Senha, usuario.SenhaHash))
            {
                return Unauthorized("E-mail ou senha inválidos.");
            }

            // 3. Se a senha estiver correta, gera o "ticket" (Token JWT)
            var token = GerarTokenJwt(usuario);

            return Ok(new { token = token });
        }


        // --- MÉTODO PRIVADO PARA GERAR O TOKEN ---
        private string GerarTokenJwt(Usuario usuario)
        {
            // Pega a chave secreta do appsettings.json
            var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            
            // Pega as credenciais de assinatura
            var credenciais = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256);

            // Define as "Claims" (informações que queremos guardar dentro do token)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()), // ID do usuário
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim("role", usuario.Role) // A "Role" (Admin ou Cliente)
            };

            // Define o token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8), // Token expira em 8 horas
                signingCredentials: credenciais);

            // Escreve o token como uma string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}