using FiapChallenge.Application.DTOs;
using FiapChallenge.Domain.Entities;
using FiapChallenge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace FiapChallenge.Application.Services
{
    public class AlunoService : IAlunoService
    {
        private readonly ApplicationDbContext _context;

        public AlunoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<AlunoResponseDto> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Alunos.AsQueryable();

            var totalCount = await query.CountAsync();

            var alunos = await query
                .OrderBy(a => a.Nome)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AlunoResponseDto
                {
                    Id = a.Id,
                    Nome = a.Nome,
                    DataNascimento = a.DataNascimento,
                    Cpf = a.Cpf,
                    Email = a.Email,
                    DataCadastro = a.DataCadastro
                })
                .ToListAsync();

            return (alunos, totalCount);
        }

        public async Task<AlunoResponseDto?> GetByIdAsync(int id)
        {
            var aluno = await _context.Alunos.FindAsync(id);

            if (aluno == null)
                return null;

            return new AlunoResponseDto
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                DataNascimento = aluno.DataNascimento,
                Cpf = aluno.Cpf,
                Email = aluno.Email,
                DataCadastro = aluno.DataCadastro
            };
        }

        public async Task<IEnumerable<AlunoResponseDto>> SearchByNameAsync(string nome)
        {
            return await _context.Alunos
                .Where(a => a.Nome.Contains(nome))
                .OrderBy(a => a.Nome)
                .Select(a => new AlunoResponseDto
                {
                    Id = a.Id,
                    Nome = a.Nome,
                    DataNascimento = a.DataNascimento,
                    Cpf = a.Cpf,
                    Email = a.Email,
                    DataCadastro = a.DataCadastro
                })
                .ToListAsync();
        }

        public async Task<AlunoResponseDto?> SearchByCpfAsync(string cpf)
        {
            // Remove formatação do CPF
            cpf = Regex.Replace(cpf, @"[^\d]", "");

            var aluno = await _context.Alunos
                .FirstOrDefaultAsync(a => a.Cpf == cpf);

            if (aluno == null)
                return null;

            return new AlunoResponseDto
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                DataNascimento = aluno.DataNascimento,
                Cpf = aluno.Cpf,
                Email = aluno.Email,
                DataCadastro = aluno.DataCadastro
            };
        }

        public async Task<AlunoResponseDto> CreateAsync(AlunoCreateDto dto)
        {
            // Remove formatação do CPF
            var cpfLimpo = Regex.Replace(dto.Cpf, @"[^\d]", "");

            // Verificar se CPF já existe
            if (await _context.Alunos.AnyAsync(a => a.Cpf == cpfLimpo))
                throw new InvalidOperationException("CPF já cadastrado");

            // Verificar se email já existe
            if (await _context.Alunos.AnyAsync(a => a.Email == dto.Email))
                throw new InvalidOperationException("E-mail já cadastrado");

            var aluno = new Aluno
            {
                Nome = dto.Nome,
                DataNascimento = dto.DataNascimento,
                Cpf = cpfLimpo,
                Email = dto.Email,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha)
            };

            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            return new AlunoResponseDto
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                DataNascimento = aluno.DataNascimento,
                Cpf = aluno.Cpf,
                Email = aluno.Email,
                DataCadastro = aluno.DataCadastro
            };
        }

        public async Task<AlunoResponseDto?> UpdateAsync(int id, AlunoUpdateDto dto)
        {
            var aluno = await _context.Alunos.FindAsync(id);

            if (aluno == null)
                return null;

            // Verificar se o novo email já existe em outro aluno
            if (await _context.Alunos.AnyAsync(a => a.Email == dto.Email && a.Id != id))
                throw new InvalidOperationException("E-mail já cadastrado para outro aluno");

            aluno.Nome = dto.Nome;
            aluno.DataNascimento = dto.DataNascimento;
            aluno.Email = dto.Email;

            await _context.SaveChangesAsync();

            return new AlunoResponseDto
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                DataNascimento = aluno.DataNascimento,
                Cpf = aluno.Cpf,
                Email = aluno.Email,
                DataCadastro = aluno.DataCadastro
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var aluno = await _context.Alunos.FindAsync(id);

            if (aluno == null)
                return false;

            _context.Alunos.Remove(aluno);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}