using FiapChallenge.Application.DTOs;
using FiapChallenge.Domain.Entities;
using FiapChallenge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FiapChallenge.Application.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly ApplicationDbContext _context;

        public MatriculaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MatriculaResponseDto> CreateAsync(MatriculaCreateDto dto)
        {
            // Verificar se aluno existe
            var aluno = await _context.Alunos.FindAsync(dto.AlunoId);
            if (aluno == null)
                throw new InvalidOperationException("Aluno não encontrado");

            // Verificar se turma existe
            var turma = await _context.Turmas.FindAsync(dto.TurmaId);
            if (turma == null)
                throw new InvalidOperationException("Turma não encontrada");

            // Verificar se aluno já está matriculado na turma
            var matriculaExistente = await _context.Matriculas
                .AnyAsync(m => m.AlunoId == dto.AlunoId && m.TurmaId == dto.TurmaId);

            if (matriculaExistente)
                throw new InvalidOperationException("Aluno já está matriculado nesta turma");

            var matricula = new Matricula
            {
                AlunoId = dto.AlunoId,
                TurmaId = dto.TurmaId
            };

            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            return new MatriculaResponseDto
            {
                Id = matricula.Id,
                AlunoId = matricula.AlunoId,
                NomeAluno = aluno.Nome,
                TurmaId = matricula.TurmaId,
                NomeTurma = turma.Nome,
                DataMatricula = matricula.DataMatricula
            };
        }

        public async Task<MatriculaResponseDto?> GetByIdAsync(int id)
        {
            var matricula = await _context.Matriculas
                .Include(m => m.Aluno)
                .Include(m => m.Turma)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (matricula == null)
                return null;

            return new MatriculaResponseDto
            {
                Id = matricula.Id,
                AlunoId = matricula.AlunoId,
                NomeAluno = matricula.Aluno.Nome,
                TurmaId = matricula.TurmaId,
                NomeTurma = matricula.Turma.Nome,
                DataMatricula = matricula.DataMatricula
            };
        }

        public async Task<IEnumerable<MatriculaResponseDto>> GetAlunosByTurmaAsync(int turmaId)
        {
            return await _context.Matriculas
                .Include(m => m.Aluno)
                .Include(m => m.Turma)
                .Where(m => m.TurmaId == turmaId)
                .OrderBy(m => m.Aluno.Nome)
                .Select(m => new MatriculaResponseDto
                {
                    Id = m.Id,
                    AlunoId = m.AlunoId,
                    NomeAluno = m.Aluno.Nome,
                    TurmaId = m.TurmaId,
                    NomeTurma = m.Turma.Nome,
                    DataMatricula = m.DataMatricula
                })
                .ToListAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var matricula = await _context.Matriculas.FindAsync(id);

            if (matricula == null)
                throw new InvalidOperationException("Matrícula não encontrada");

            _context.Matriculas.Remove(matricula);
            await _context.SaveChangesAsync();
        }
    }
}
