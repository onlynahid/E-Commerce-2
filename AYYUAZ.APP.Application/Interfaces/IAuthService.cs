using AYYUAZ.APP.Application.Dtos;
using System.Security.Claims;
namespace AYYUAZ.APP.Application.Interfaces
{
    public interface IAuthService 
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<bool> ValidateTokenAsync(string token);
        Task<ClaimsPrincipal?> GetPrincipalFromTokenAsync(string token);
        Task<TokenDto> GenerateTokenAsync(string userId);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
        Task<bool> ChangeEmailAsync(string userId, ChangeEmailDto changeEmailDto);
    }
}