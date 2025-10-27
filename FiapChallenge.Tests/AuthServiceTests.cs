using FiapChallenge.Application.DTOs;
using FiapChallenge.Application.Services;
using FiapChallenge.Domain.Entities;
using FiapChallenge.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FiapChallenge.Tests
{
    public class AuthServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            // Configurar banco de dados em memória
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                { "JwtSettings:SecretKey", "ChaveSecretaSuperSeguraParaJWT2024!@#$%" },
                { "JwtSettings:Issuer", "FiapChallengeAPI" },
                { "JwtSettings:Audience", "FiapChallengeClient" }
            }!);
            _configuration = configurationBuilder.Build();

            _authService = new AuthService(_context, _configuration);
        }

        [Fact]
        public async Task Login_ComCredenciaisValidas_DeveRetornarToken()
        {
            // Arrange
            var usuario = new Usuario
            {
                Email = "admin@fiap.com.br",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                DataCadastro = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var loginDto = new LoginDto
            {
                Email = "admin@fiap.com.br",
                Senha = "Admin@123"
            };

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
            result.Email.Should().Be(loginDto.Email);
            result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
        }

        [Fact]
        public async Task Login_ComEmailInvalido_DeveLancarException()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "invalido@fiap.com.br",
                Senha = "Admin@123"
            };

            // Act
            Func<Task> act = async () => await _authService.LoginAsync(loginDto);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("E-mail ou senha inválidos");
        }

        [Fact]
        public async Task Login_ComSenhaInvalida_DeveLancarException()
        {
            // Arrange
            var usuario = new Usuario
            {
                Email = "admin@fiap.com.br",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                DataCadastro = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var loginDto = new LoginDto
            {
                Email = "admin@fiap.com.br",
                Senha = "SenhaErrada@123"
            };

            // Act
            Func<Task> act = async () => await _authService.LoginAsync(loginDto);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("E-mail ou senha inválidos");
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
