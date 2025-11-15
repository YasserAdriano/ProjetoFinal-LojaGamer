using LojaGamerApi.Controllers;
using LojaGamerApi.Data;
using LojaGamerApi.Dtos;
using LojaGamerApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace LojaGamerApi.Tests
{
    public class AuthControllerTests
    {
        private LojaGamerContext _context;
        private IConfiguration _configuration;

        public AuthControllerTests()
        {
            var options = new DbContextOptionsBuilder<LojaGamerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new LojaGamerContext(options);

            var inMemorySettings = new Dictionary<string, string?> {
                {"Jwt:Key", "minha-chave-secreta-de-teste-super-longa-com-mais-de-32-bytes"},
                {"Jwt:Issuer", "teste.com"},
                {"Jwt:Audience", "teste.com"},
            };
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        // --- TESTE 1 ---
        [Fact]
        public async Task Register_DeveFalhar_QuandoEmailJaExiste()
        {
            var usuarioExistente = new Usuario { Nome = "Usuario Antigo", Email = "email@teste.com", SenhaHash = "123", Role = "Cliente" };
            _context.Usuarios.Add(usuarioExistente);
            await _context.SaveChangesAsync();
            var controller = new AuthController(_context, _configuration);
            var novoUsuarioDto = new RegisterDto { Nome = "Usuario Novo", Email = "email@teste.com", Senha = "senha123" };
            var resultado = await controller.Register(novoUsuarioDto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.Equal("Este e-mail já está em uso.", badRequestResult.Value);
        }

        // --- TESTE 2 ---
        [Fact]
        public async Task Register_DeveCriarUsuario_QuandoEmailENovo()
        {
            var controller = new AuthController(_context, _configuration);
            var novoUsuarioDto = new RegisterDto { Nome = "Novo Usuario Teste", Email = "emailnovo@teste.com", Senha = "senha123" };
            var resultado = await controller.Register(novoUsuarioDto);
            Assert.IsType<OkObjectResult>(resultado);
            var usuarioSalvo = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == "emailnovo@teste.com");
            Assert.NotNull(usuarioSalvo);
            Assert.Equal("Novo Usuario Teste", usuarioSalvo.Nome);
        }

        // --- TESTE 3 ---
        [Fact]
        public async Task Login_DeveFalhar_QuandoSenhaEstaErrada()
        {
            string senhaHash = BCrypt.Net.BCrypt.HashPassword("senhaCorreta");
            var usuarioExistente = new Usuario { Nome = "Usuario Login", Email = "login@teste.com", SenhaHash = senhaHash, Role = "Cliente" };
            _context.Usuarios.Add(usuarioExistente);
            await _context.SaveChangesAsync();
            var controller = new AuthController(_context, _configuration);
            var loginDto = new LoginDto { Email = "login@teste.com", Senha = "senhaErrada" };
            var resultado = await controller.Login(loginDto);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(resultado);
            Assert.Equal("E-mail ou senha inválidos.", unauthorizedResult.Value);
        }

        // --- TESTE 4 ---
        [Fact]
        public async Task Login_DeveRetornarToken_QuandoCredenciaisEstaoCorretas()
        {
            string senhaHash = BCrypt.Net.BCrypt.HashPassword("senhaCorreta123");
            var usuarioExistente = new Usuario { Id = 1, Nome = "Usuario Login Certo", Email = "logincerto@teste.com", SenhaHash = senhaHash, Role = "Cliente" };
            _context.Usuarios.Add(usuarioExistente);
            await _context.SaveChangesAsync();
            var controller = new AuthController(_context, _configuration);
            var loginDto = new LoginDto { Email = "logincerto@teste.com", Senha = "senhaCorreta123" };
            var resultado = await controller.Login(loginDto);
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            
            var tokenObject = okResult.Value!.GetType().GetProperty("token")!.GetValue(okResult.Value, null);
            Assert.NotNull(tokenObject);
            Assert.IsType<string>(tokenObject);
        }

        // --- TESTE 5  ---
        [Fact]
        public async Task GetProdutoPorId_DeveRetornar404_QuandoProdutoNaoExiste()
        {
            // --- 1. ARRANGE ---
            var controller = new ProdutosController(_context);

            // --- 2. ACT ---
            var resultado = await controller.GetProdutoPorId(999);

            // --- 3. ASSERT ---
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado);
            Assert.Equal("Produto não encontrado.", notFoundResult.Value);
        }
    }
}