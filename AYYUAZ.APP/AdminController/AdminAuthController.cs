using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Infrastructure.ApplicationUser;
using AYYUAZ.APP.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AYYUAZ.APP.AdminController;
[ApiController]
[Route("api/[controller]")]
public class AdminAuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AdminAuthController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;
    public AdminAuthController(
        IAuthService authService,
        ILogger<AdminAuthController> logger,
        UserManager<User> userManager,
        IJwtService jwtService)
    {
        _authService = authService;
        _logger = logger;
        _userManager = userManager;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        _logger.LogInformation("Login request received");
        
        var result = await _authService.LoginAsync(loginDto);
        return Ok(result);
    }
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        _logger.LogInformation("Registration request received");

        var result = await _authService.RegisterAsync(registerDto);
        return Ok(result);
    }
    [HttpPost("validate-token")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenValidationResponseDto>> ValidateToken([FromBody] TokenValidationRequest request)
    {
        _logger.LogInformation("Token validation requested");

        var isValid = await _authService.ValidateTokenAsync(request.Token);
        return Ok(new { 
            success = isValid, 
            message = isValid ? "Token is valid" : "Token is invalid" 
        });
    }
    [HttpGet("me")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public ActionResult<CurrentUserDto> GetCurrentUser()
    {
        _logger.LogInformation("Get current user requested");

        var user = new CurrentUserDto
        {
            UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            Username = User.FindFirst(ClaimTypes.Name)?.Value,
            Email = User.FindFirst(ClaimTypes.Email)?.Value,
            Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList(),
            IsAdmin = User.FindFirst("isAdmin")?.Value,
            IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
            Claims = User.Claims
                .Select(c => new ClaimDto { Type = c.Type, Value = c.Value })
                .ToList()
        };
        return Ok(user);
    }
    //[HttpPost("debug/decode-token")]
    //[AllowAnonymous]
    //public ActionResult<object> DecodeToken([FromBody] TokenValidationRequest request)
    //{
    //    if (string.IsNullOrEmpty(request.Token))
    //    {
    //        return BadRequest(new { error = "Token is required" });
    //    }

    //    var handler = new JwtSecurityTokenHandler();

    //    if (!handler.CanReadToken(request.Token))
    //    {
    //        return BadRequest(new { error = "Invalid JWT token format" });
    //    }

    //    var jsonToken = handler.ReadJwtToken(request.Token);

    //    var claims = jsonToken.Claims.Select(c => new { c.Type, c.Value }).ToList();

    //    return Ok(new
    //    {
    //        header = jsonToken.Header,
    //        payload = new
    //        {
    //            issuer = jsonToken.Issuer,
    //            audience = jsonToken.Audiences,
    //            expiry = jsonToken.ValidTo,
    //            issuedAt = jsonToken.IssuedAt,
    //            notBefore = jsonToken.ValidFrom
    //        },
    //        claims,
    //        isExpired = jsonToken.ValidTo < DateTime.UtcNow,
    //        timeUntilExpiry = jsonToken.ValidTo - DateTime.UtcNow
    //    });
    //}
    [HttpGet("debug")]
    [AllowAnonymous]
    public ActionResult Debug()
    {
        return Ok(new
        {
            timestamp = DateTime.UtcNow,
            authServiceRegistered = _authService != null,
            jwtServiceRegistered = _jwtService != null,
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            message = "AuthController debug info"
        });
    }
    [HttpPost("change-password")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found in token");
        }

        var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);

        return Ok(new
        {
            success = result,
            message = result ? "Password changed successfully" : "Password change failed",
            timestamp = DateTime.UtcNow
        });
    }
    [HttpPost("change-email")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult> ChangeEmail([FromBody] ChangeEmailDto changeEmailDto)
    {
        _logger.LogInformation("Email change request received");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Email change failed - user ID not found in token");
            return Unauthorized(new
            {
                success = false,
                message = "User ID not found in token",
                timestamp = DateTime.UtcNow
            });
        }

        var result = await _authService.ChangeEmailAsync(userId, changeEmailDto);

        _logger.LogInformation("Email change result: {Result}", result);

        return Ok(new
        {
            success = result,
            message = result ? "Email changed successfully" : "Email change failed",
            newEmail = result ? changeEmailDto.NewEmail : null,
            timestamp = DateTime.UtcNow
        });
    }
}
public class TokenValidationRequest
{
    [Required]
    public string Token { get; set; } = string.Empty;
}




