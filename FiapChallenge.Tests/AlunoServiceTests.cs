using FiapChallenge.Application.DTOs;
using FiapChallenge.Application.Services;
using FiapChallenge.Domain.Entities;
using FiapChallenge.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FiapChallenge.Tests
{
    public class AlunoServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly AlunoService _alunoService;

        public AlunoServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _alunoService = new AlunoService(_context);
        }

        [Fact]
        public async Task CreateAsync_ComDadosValidos_DeveCriarAluno()
        {
            // Arrange
            var createDto = new AlunoCreateDto
            {
                Nome = "João Silva",
                DataNascimento = new DateTime(1995, 5, 15),
                Cpf = "12345678901",
                Email = "joao.silva@email.com",
                Senha = "Senha@123"
            };

            // Act
            var result = await _alunoService.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);
            result.Nome.Should().Be(createDto.Nome);
            result.Cpf.Should().Be(createDto.Cpf);
            result.Email.Should().Be(createDto.Email);

            var alunoNoBanco = await _context.Alunos.FindAsync(result.Id);
            alunoNoBanco.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateAsync_ComCpfDuplicado_DeveLancarException()
        {
            // Arrange
            var aluno = new Aluno
            {
                Nome = "Maria Santos",
                DataNascimento = new DateTime(1990, 1, 1),
                Cpf = "12345678901",
                Email = "maria@email.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Senha@123"),
                DataCadastro = DateTime.UtcNow
            };

            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            var createDto = new AlunoCreateDto
            {
                Nome = "João Silva",
                DataNascimento = new DateTime(1995, 5, 15),
                Cpf = "12345678901",
                Email = "joao.silva@email.com",
                Senha = "Senha@123"
            };

            // Act
            Func<Task> act = async () => await _alunoService.CreateAsync(createDto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*CPF*");
        }

        [Fact]
        public async Task CreateAsync_ComEmailDuplicado_DeveLancarException()
        {
            // Arrange
            var aluno = new Aluno
            {
                Nome = "Maria Santos",
                DataNascimento = new DateTime(1990, 1, 1),
                Cpf = "98765432100",
                Email = "joao.silva@email.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Senha@123"),
                DataCadastro = DateTime.UtcNow
            };

            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            var createDto = new AlunoCreateDto
            {
                Nome = "João Silva",
                DataNascimento = new DateTime(1995, 5, 15),
                Cpf = "12345678901",
                Email = "joao.silva@email.com",
                Senha = "Senha@123"
            };

            // Act
            Func<Task> act = async () => await _alunoService.CreateAsync(createDto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*e-mail*");
        }

        [Fact]
        public async Task GetByIdAsync_ComIdValido_DeveRetornarAluno()
        {
            // Arrange
            var aluno = new Aluno
            {
                Nome = "João Silva",
                DataNascimento = new DateTime(1995, 5, 15),
                Cpf = "12345678901",
                Email = "joao.silva@email.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Senha@123"),
                DataCadastro = DateTime.UtcNow
            };

            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            // Act
            var result = await _alunoService.GetByIdAsync(aluno.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(aluno.Id);
            result.Nome.Should().Be("João Silva");
        }

        [Fact]
        public async Task GetByIdAsync_ComIdInvalido_DeveRetornarNull()
        {
            // Act
            var result = await _alunoService.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarListaPaginada()
        {
            // Arrange
            var alunos = new List<Aluno>
            {
                new Aluno
                {
                    Nome = "Aluno A",
                    DataNascimento = new DateTime(1995, 1, 1),
                    Cpf = "11111111111",
                    Email = "alunoa@email.com",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("Senha@123"),
                    DataCadastro = DateTime.UtcNow
                },
                new Aluno
                {
                    Nome = "Aluno B",
                    DataNascimento = new DateTime(1996, 2, 2),
                    Cpf = "22222222222",
                    Email = "alunob@email.com",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("Senha@123"),
                    DataCadastro = DateTime.UtcNow
                }
            };

            _context.Alunos.AddRange(alunos);
            await _context.SaveChangesAsync();

            // Act
            var (result, total) = await _alunoService.GetAllAsync(1, 10);

            // Assert
            result.Should().NotBeNull();
            result!.Should().HaveCount(2);
            total.Should().Be(2);
        }

        [Fact]
        public async Task UpdateAsync_ComDadosValidos_DeveAtualizarAluno()
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

            var updateDto = new AlunoUpdateDto
            {
                Nome = "João Silva Atualizado",
                DataNascimento = new DateTime(1995, 5, 15),
                Email = "joao.novo@email.com"
            };

            // Act
            var result = await _alunoService.UpdateAsync(aluno.Id, updateDto);

            // Assert
            result.Should().NotBeNull();
            result!.Nome.Should().Be(updateDto.Nome);
            result.Email.Should().Be(updateDto.Email);
        }

        [Fact]
        public async Task DeleteAsync_ComIdValido_DeveDeletarAluno()
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

            // Act
            await _alunoService.DeleteAsync(aluno.Id);

            // Assert
            var alunoNoBanco = await _context.Alunos.FindAsync(aluno.Id);
            alunoNoBanco.Should().BeNull();
        }

        [Fact]
        public async Task SearchByNameAsync_ComNomeValido_DeveRetornarAlunos()
        {
            // Arrange
            var alunos = new List<Aluno>
            {
                new Aluno
                {
                    Nome = "João Silva",
                    DataNascimento = new DateTime(1995, 1, 1),
                    Cpf = "11111111111",
                    Email = "joao1@email.com",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("Senha@123"),
                    DataCadastro = DateTime.UtcNow
                },
                new Aluno
                {
                    Nome = "João Santos",
                    DataNascimento = new DateTime(1996, 2, 2),
                    Cpf = "22222222222",
                    Email = "joao2@email.com",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("Senha@123"),
                    DataCadastro = DateTime.UtcNow
                },
                new Aluno
                {
                    Nome = "Maria Silva",
                    DataNascimento = new DateTime(1997, 3, 3),
                    Cpf = "33333333333",
                    Email = "maria@email.com",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("Senha@123"),
                    DataCadastro = DateTime.UtcNow
                }
            };

            _context.Alunos.AddRange(alunos);
            await _context.SaveChangesAsync();

            // Act
            var result = await _alunoService.SearchByNameAsync("João");

            // Assert
            result.Should().HaveCount(2);
            result.All(a => a.Nome.Contains("João")).Should().BeTrue();
        }

        [Fact]
        public async Task GetByCpfAsync_ComCpfValido_DeveRetornarAluno()
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

            // Act
            var result = await _alunoService.SearchByCpfAsync("12345678901");

            // Assert
            result.Should().NotBeNull();
            result!.Cpf.Should().Be("12345678901");
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
