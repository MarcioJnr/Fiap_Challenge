using FiapChallenge.Application.DTOs;

namespace FiapChallenge.Application.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
    }
}