using FiapChallenge.Application.DTOs;
using FiapChallenge.Domain.Entities;
using FiapChallenge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FiapChallenge.Application.Services
{
    public class TurmaService : ITurmaService
    {
        private readonly ApplicationDbContext _context;

        public TurmaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<TurmaResponseDto> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Turmas.AsQueryable();

            var totalCount = await query.CountAsync();

            var turmas = await query
                .Include(t => t.Matriculas)
                .OrderBy(t => t.Nome)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TurmaResponseDto
                {
                    Id = t.Id,
                    Nome = t.Nome,
                    Descricao = t.Descricao,
                    QuantidadeAlunos = t.Matriculas.Count,
                    DataCadastro = t.DataCadastro
                })
                .ToListAsync();

            return (turmas, totalCount);
        }

        public async Task<TurmaResponseDto?> GetByIdAsync(int id)
        {
            var turma = await _context.Turmas
                .Include(t => t.Matriculas)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (turma == null)
                return null;

            return new TurmaResponseDto
            {
                Id = turma.Id,
                Nome = turma.Nome,
                Descricao = turma.Descricao,
                QuantidadeAlunos = turma.Matriculas.Count,
                DataCadastro = turma.DataCadastro
            };
        }

        public async Task<TurmaResponseDto> CreateAsync(TurmaCreateDto dto)
        {
            var turma = new Turma
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao
            };

            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            return new TurmaResponseDto
            {
                Id = turma.Id,
                Nome = turma.Nome,
                Descricao = turma.Descricao,
                QuantidadeAlunos = 0,
                DataCadastro = turma.DataCadastro
            };
        }

        public async Task<TurmaResponseDto?> UpdateAsync(int id, TurmaUpdateDto dto)
        {
            var turma = await _context.Turmas
                .Include(t => t.Matriculas)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (turma == null)
                return null;

            turma.Nome = dto.Nome;
            turma.Descricao = dto.Descricao;

            await _context.SaveChangesAsync();

            return new TurmaResponseDto
            {
                Id = turma.Id,
                Nome = turma.Nome,
                Descricao = turma.Descricao,
                QuantidadeAlunos = turma.Matriculas.Count,
                DataCadastro = turma.DataCadastro
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var turma = await _context.Turmas.FindAsync(id);

            if (turma == null)
                return false;

            _context.Turmas.Remove(turma);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}