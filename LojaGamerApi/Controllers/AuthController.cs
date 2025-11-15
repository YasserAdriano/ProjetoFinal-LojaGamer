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
    [Route("api/[controller]")] 
    public class AuthController : ControllerBase
    {
        private readonly LojaGamerContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(LojaGamerContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // --- ENDPOINT DE REGISTRO ---
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == registerDto.Email))
            {
                return BadRequest("Este e-mail já está em uso.");
            }

            string senhaHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Senha);

            var usuario = new Usuario
            {
                Nome = registerDto.Nome,
                Email = registerDto.Email,
                SenhaHash = senhaHash,
                Role = "Cliente" 
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync(); 

            return Ok(new { message = "Usuário registrado com sucesso!" });
        }

        // --- ENDPOINT DE LOGIN ---
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(loginDto.Senha, usuario.SenhaHash))
            {
                return Unauthorized("E-mail ou senha inválidos.");
            }

            var token = GerarTokenJwt(usuario);

            return Ok(new { token = token });
        }


        private string GerarTokenJwt(Usuario usuario)
        {
            var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            
            var credenciais = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()), 
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim("role", usuario.Role) 
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8), 
                signingCredentials: credenciais);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}