using FiapChallenge.Application.DTOs;
using FiapChallenge.Application.Services;
using FiapChallenge.Domain.Entities;
using FiapChallenge.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FiapChallenge.Tests
{
    public class TurmaServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly TurmaService _turmaService;

        public TurmaServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _turmaService = new TurmaService(_context);
        }

        [Fact]
        public async Task CreateAsync_ComDadosValidos_DeveCriarTurma()
        {
            // Arrange
            var createDto = new TurmaCreateDto
            {
                Nome = "Desenvolvimento .NET",
                Descricao = "Turma focada em desenvolvimento de aplicações com .NET Core"
            };

            // Act
            var result = await _turmaService.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Nome.Should().Be(createDto.Nome);
            result.Descricao.Should().Be(createDto.Descricao);

            var turmaNoBanco = await _context.Turmas.FindAsync(result.Id);
            turmaNoBanco.Should().NotBeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ComIdValido_DeveRetornarTurma()
        {
            // Arrange
            var turma = new Turma
            {
                Nome = "Desenvolvimento .NET",
                Descricao = "Turma focada em desenvolvimento",
                DataCadastro = DateTime.UtcNow
            };

            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            // Act
            var result = await _turmaService.GetByIdAsync(turma.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(turma.Id);
            result.Nome.Should().Be("Desenvolvimento .NET");
        }

        [Fact]
        public async Task UpdateAsync_ComDadosValidos_DeveAtualizarTurma()
        {
            // Arrange
            var turma = new Turma
            {
                Nome = "Turma Original",
                Descricao = "Descrição Original",
                DataCadastro = DateTime.UtcNow
            };

            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            var updateDto = new TurmaUpdateDto
            {
                Nome = "Turma Atualizada",
                Descricao = "Descrição Atualizada"
            };

            // Act
            var result = await _turmaService.UpdateAsync(turma.Id, updateDto);

            // Assert
            result.Should().NotBeNull();
            result!.Nome.Should().Be(updateDto.Nome);
            result.Descricao.Should().Be(updateDto.Descricao);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
