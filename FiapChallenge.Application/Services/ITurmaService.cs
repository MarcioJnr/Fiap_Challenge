using FiapChallenge.Application.DTOs;

namespace FiapChallenge.Application.Services
{
    public interface ITurmaService
    {
        Task<(IEnumerable<TurmaResponseDto> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
        Task<TurmaResponseDto?> GetByIdAsync(int id);
        Task<TurmaResponseDto> CreateAsync(TurmaCreateDto dto);
        Task<TurmaResponseDto?> UpdateAsync(int id, TurmaUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}