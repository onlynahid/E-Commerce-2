using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Application.Exceptions.AppException;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Reflection;
using System.Security.Claims;
using AYYUAZ.APP.Constants;
using AYYUAZ.APP.Infrastructure.ApplicationUser;
namespace AYYUAZ.APP.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;
        private readonly IMapper _mapper;
        public AuthService(
            UserManager<User> userManager,
            IJwtService jwtService,
            ILogger<AuthService> logger,
            IMapper mapper)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            _logger.LogInformation("Login attempt started for email: {Email}", loginDto?.Email ?? "null");

            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
            {
                _logger.LogWarning("Login failed - invalid input data");
                throw new BadRequestException(ErrorMessages.BadRequest);
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed - user not found for email: {Email}", loginDto.Email);
                throw new UnauthorizedException(ErrorMessages.Unauthorized);
            }

            _logger.LogDebug("User found for email: {Email}, UserId: {UserId}", loginDto.Email, user.Id);

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Login failed - invalid password for email: {Email}", loginDto.Email);
                throw new UnauthorizedException(ErrorMessages.Unauthorized);
            }

            _logger.LogDebug("Password verification successful for user: {UserId}", user.Id);

            var userRoles = await _userManager.GetRolesAsync(user);
            var isAdmin = userRoles.Contains("Admin");

            var tokenInfo = await _jwtService.GenerateTokenAsync(user.Id);

            _logger.LogInformation("Login successful for user: {UserId}, roles: [{Roles}]",
                user.Id, string.Join(", ", userRoles));

            var userDto = _mapper.Map<UserDto>(user);
            userDto.IsAdmin = isAdmin;

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful",
                Token = tokenInfo.AccessToken,
                TokenInfo = tokenInfo,
                User = userDto
            };
        }
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            _logger.LogInformation("Registration attempt started for email: {Email}", registerDto?.Email ?? "null");

            if (registerDto == null || string.IsNullOrEmpty(registerDto.Email) || string.IsNullOrEmpty(registerDto.Password))
            {
                _logger.LogWarning("Registration failed - invalid input data");
                throw new BadRequestException(ErrorMessages.BadRequest);
            }

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed - user already exists for email: {Email}", registerDto.Email);
                throw new BadRequestException(ErrorMessages.BadRequest);
            }

            var existingUsername = await _userManager.FindByNameAsync(registerDto.Username);
            if (existingUsername != null)
            {
                _logger.LogWarning("Registration failed - username already exists: {Username}", registerDto.Username);
                throw new BadRequestException(ErrorMessages.BadRequest);
            }

            var user = _mapper.Map<User>(registerDto);

            _logger.LogDebug("Creating new user with email: {Email}, username: {UserName}",
                registerDto.Email, user.UserName);

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));

                _logger.LogWarning("Registration failed - user creation failed for email: {Email}, errors: {Errors}",
                    registerDto.Email, errors);

                throw new BadRequestException(ErrorMessages.BadRequest);
            }

            await _userManager.AddToRoleAsync(user, "User");
            _logger.LogDebug("Assigned default role 'User' to user: {UserId}", user.Id);

            var userRoles = await _userManager.GetRolesAsync(user);
            var isAdmin = userRoles.Contains("Admin");

            var tokenInfo = await _jwtService.GenerateTokenAsync(user.Id);

            _logger.LogInformation("Registration successful for user: {UserId}, email: {Email}",
                user.Id, user.Email);

            var userDto = _mapper.Map<UserDto>(user);
            userDto.IsAdmin = isAdmin;

            return new AuthResponseDto
            {
                Success = true,
                Message = "Registration successful",
                Token = tokenInfo.AccessToken,
                TokenInfo = tokenInfo,
                User = userDto
            };
        }
        public async Task<bool> ValidateTokenAsync(string token)
        {
            _logger.LogDebug("Token validation requested");

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token validation failed - empty token");
                return false;
            }

            var principal = await _jwtService.ValidateTokenAsync(token); // Updated to use ValidateTokenAsync
            var isValid = principal != null;

            _logger.LogDebug("Token validation result: {IsValid}", isValid);

            return isValid;
        }
        public async Task<ClaimsPrincipal?> GetPrincipalFromTokenAsync(string token)
        {
            _logger.LogDebug("Getting principal from token");

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Get principal failed - empty token");
                return null;
            }

            var principal = await _jwtService.ValidateTokenAsync(token); // Updated to use ValidateTokenAsync

            _logger.LogDebug("Principal extraction result: {HasPrincipal}", principal != null);
            return principal;
        }
        public async Task<TokenDto> GenerateTokenAsync(string userId)
        {
            _logger.LogDebug("Token generation requested for user: {UserId}", userId);

            if (string.IsNullOrWhiteSpace(userId))
                throw new BadRequestException(ErrorMessages.BadRequest);

            var tokenInfo = await _jwtService.GenerateTokenAsync(userId);

            _logger.LogDebug("Token generated successfully for user: {UserId}", userId);

            return tokenInfo;
        }
        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException();

            var result = await _userManager.ChangePasswordAsync(
                user,
                changePasswordDto.CurrentPassword,
                changePasswordDto.NewPassword
            );

            if (!result.Succeeded)
            {
                _logger.LogWarning("Password change failed for user: {UserId}, errors: {Errors}",
                    userId, string.Join(", ", result.Errors.Select(e => e.Description)));

                throw new BadRequestException(ErrorMessages.BadRequest);
            }

            _logger.LogInformation("Password changed successfully for user: {UserId}", userId);

            return true;
        }
        public async Task<bool> ChangeEmailAsync(string userId, ChangeEmailDto changeEmailDto)
        {
            _logger.LogInformation("Email change attempt started for user: {UserId}", userId);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Email change failed - user not found: {UserId}", userId);
                throw new NotFoundException();
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, changeEmailDto.CurrentPassword);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Email change failed - invalid password for user: {UserId}", userId);
                throw new UnauthorizedException(ErrorMessages.Unauthorized);
            }

            var existingUser = await _userManager.FindByEmailAsync(changeEmailDto.NewEmail);
            if (existingUser != null && existingUser.Id != userId)
            {
                _logger.LogWarning("Email change failed - email already in use: {NewEmail}", changeEmailDto.NewEmail);
                throw new BadRequestException(ErrorMessages.BadRequest);
            }

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, changeEmailDto.NewEmail);
            var result = await _userManager.ChangeEmailAsync(user, changeEmailDto.NewEmail, token);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Email change failed for user: {UserId}, errors: {Errors}",
                    userId, string.Join(", ", result.Errors.Select(e => e.Description)));

                throw new BadRequestException(ErrorMessages.BadRequest);
            }

            user.UserName = changeEmailDto.NewEmail;
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                _logger.LogWarning("Email changed but username update failed for user: {UserId}, errors: {Errors}",
                    userId, string.Join(", ", updateResult.Errors.Select(e => e.Description)));
            }

            _logger.LogInformation("Email changed successfully for user: {UserId}, new email: {NewEmail}",
                userId, changeEmailDto.NewEmail);

            return true;
        }
    }
}

