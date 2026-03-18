using AYYUAZ.APP.Application.Dtos;
using System.Security.Claims;

namespace AYYUAZ.APP.Application.Interfaces
{
    public interface IJwtService 
    {
        Task<TokenDto> GenerateTokenAsync(string userId);
        ClaimsPrincipal? ValidateToken(string token);
        Task<ClaimsPrincipal?> ValidateTokenAsync(string token); // <-- Added async version
        Task<string> GenerateRefreshTokenAsync();
    }
}