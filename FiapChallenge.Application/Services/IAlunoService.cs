using FiapChallenge.Application.DTOs;

namespace FiapChallenge.Application.Services
{
    public interface IAlunoService
    {
        Task<(IEnumerable<AlunoResponseDto> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
        Task<AlunoResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<AlunoResponseDto>> SearchByNameAsync(string nome);
        Task<AlunoResponseDto?> SearchByCpfAsync(string cpf);
        Task<AlunoResponseDto> CreateAsync(AlunoCreateDto dto);
        Task<AlunoResponseDto?> UpdateAsync(int id, AlunoUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}