using FiapChallenge.Application.DTOs;
using FiapChallenge.Application.Services;
using FiapChallenge.Domain.Entities;
using FiapChallenge.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FiapChallenge.Tests
{
    public class MatriculaServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly MatriculaService _matriculaService;

        public MatriculaServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _matriculaService = new MatriculaService(_context);
        }

        [Fact]
        public async Task CreateAsync_ComDadosValidos_DeveCriarMatricula()
        {
            // Arrange
            var aluno = new Aluno
            {
                Nome = "João Silva",
                DataNascimento = new DateTime(1995, 5, 15),
                Cpf = "12345678901",
                Email = "joao@email.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Senha@123"),
                DataCadastro = DateTime.UtcNow
            };

            var turma = new Turma
            {
                Nome = "Turma A",
                Descricao = "Descrição A",
                DataCadastro = DateTime.UtcNow
            };

            _context.Alunos.Add(aluno);
            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            var createDto = new MatriculaCreateDto
            {
                AlunoId = aluno.Id,
                TurmaId = turma.Id
            };

            // Act
            var result = await _matriculaService.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.AlunoId.Should().Be(aluno.Id);
            result.TurmaId.Should().Be(turma.Id);

            var matriculaNoBanco = await _context.Matriculas.FindAsync(result.Id);
            matriculaNoBanco.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateAsync_ComAlunoInexistente_DeveLancarException()
        {
            // Arrange
            var turma = new Turma
            {
                Nome = "Turma A",
                Descricao = "Descrição A",
                DataCadastro = DateTime.UtcNow
            };

            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            var createDto = new MatriculaCreateDto
            {
                AlunoId = 999,
                TurmaId = turma.Id
            };

            // Act
            Func<Task> act = async () => await _matriculaService.CreateAsync(createDto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Aluno*");
        }

        [Fact]
        public async Task CreateAsync_ComTurmaInexistente_DeveLancarException()
        {
            // Arrange
            var aluno = new Aluno
            {
                Nome = "João Silva",
                DataNascimento = new DateTime(1995, 5, 15),
                Cpf = "12345678901",
                Email = "joao@email.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Senha@123"),
                DataCadastro = DateTime.UtcNow
            };

            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            var createDto = new MatriculaCreateDto
            {
                AlunoId = aluno.Id,
                TurmaId = 999
            };

            // Act
            Func<Task> act = async () => await _matriculaService.CreateAsync(createDto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Turma*");
        }

        [Fact]
        public async Task CreateAsync_ComMatriculaDuplicada_DeveLancarException()
        {
            // Arrange
            var aluno = new Aluno
            {
                Nome = "João Silva",
                DataNascimento = new DateTime(1995, 5, 15),
                Cpf = "12345678901",
                Email = "joao@email.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Senha@123"),
                DataCadastro = DateTime.UtcNow
            };

            var turma = new Turma
            {
                Nome = "Turma A",
                Descricao = "Descrição A",
                DataCadastro = DateTime.UtcNow
            };

            _context.Alunos.Add(aluno);
            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            var matricula = new Matricula
            {
                AlunoId = aluno.Id,
                TurmaId = turma.Id,
                DataMatricula = DateTime.UtcNow
            };

            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            var createDto = new MatriculaCreateDto
            {
                AlunoId = aluno.Id,
                TurmaId = turma.Id
            };

            // Act
            Func<Task> act = async () => await _matriculaService.CreateAsync(createDto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*matriculado*");
        }

        [Fact]
        public async Task GetByTurmaIdAsync_ComIdValido_DeveRetornarAlunos()
        {
            // Arrange
            var aluno1 = new Aluno
            {
                Nome = "João Silva",
                DataNascimento = new DateTime(1995, 5, 15),
                Cpf = "11111111111",
                Email = "joao@email.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Senha@123"),
                DataCadastro = DateTime.UtcNow
            };

            var aluno2 = new Aluno
            {
                Nome = "Maria Santos",
                DataNascimento = new DateTime(1996, 6, 16),
                Cpf = "22222222222",
                Email = "maria@email.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Senha@123"),
                DataCadastro = DateTime.UtcNow
            };

            var turma = new Turma
            {
                Nome = "Turma A",
                Descricao = "Descrição A",
                DataCadastro = DateTime.UtcNow
            };

            _context.Alunos.AddRange(aluno1, aluno2);
            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            var matricula1 = new Matricula
            {
                AlunoId = aluno1.Id,
                TurmaId = turma.Id,
                DataMatricula = DateTime.UtcNow
            };

            var matricula2 = new Matricula
            {
                AlunoId = aluno2.Id,
                TurmaId = turma.Id,
                DataMatricula = DateTime.UtcNow
            };

            _context.Matriculas.AddRange(matricula1, matricula2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _matriculaService.GetAlunosByTurmaAsync(turma.Id);

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task DeleteAsync_ComIdValido_DeveDeletarMatricula()
        {
            // Arrange
            var aluno = new Aluno
            {
                Nome = "João Silva",
                DataNascimento = new DateTime(1995, 5, 15),
                Cpf = "12345678901",
                Email = "joao@email.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Senha@123"),
                DataCadastro = DateTime.UtcNow
            };

            var turma = new Turma
            {
                Nome = "Turma A",
                Descricao = "Descrição A",
                DataCadastro = DateTime.UtcNow
            };

            var matricula = new Matricula
            {
                AlunoId = aluno.Id,
                TurmaId = turma.Id,
                DataMatricula = DateTime.UtcNow
            };

            _context.Alunos.Add(aluno);
            _context.Turmas.Add(turma);
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            // Act
            await _matriculaService.DeleteAsync(matricula.Id);

            // Assert
            var matriculaNoBanco = await _context.Matriculas.FindAsync(matricula.Id);
            matriculaNoBanco.Should().BeNull();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
