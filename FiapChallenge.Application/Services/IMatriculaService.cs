using FiapChallenge.Application.DTOs;

namespace FiapChallenge.Application.Services
{
    public interface IMatriculaService
    {
        Task<MatriculaResponseDto> CreateAsync(MatriculaCreateDto dto);
        Task<MatriculaResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<MatriculaResponseDto>> GetAlunosByTurmaAsync(int turmaId);
        Task DeleteAsync(int id);
    }
}
