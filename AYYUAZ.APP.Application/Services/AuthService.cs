using AYYUAZ.APP.Application.Dtos;
using AYYUAZ.APP.Application.Interfaces;
using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Application.Exceptions.AppException;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Security.Claims;

namespace AYYUAZ.APP.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<User> userManager,
            IJwtService jwtService,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                _logger.LogInformation("Login attempt started for email: {Email}", loginDto?.Email ?? "null");

                if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                {
                    _logger.LogWarning("Login failed - invalid input data");
                    throw new BadRequestException();
                }

                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    _logger.LogWarning("Login failed - user not found for email: {Email}", loginDto.Email);
                    throw new UnauthorizedException();
                }

                _logger.LogDebug("User found for email: {Email}, UserId: {UserId}", loginDto.Email, user.Id);

                var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
                if (!isPasswordValid)
                {
                    _logger.LogWarning("Login failed - invalid password for email: {Email}", loginDto.Email);
                    throw new UnauthorizedException();
                }

                _logger.LogDebug("Password verification successful for user: {UserId}", user.Id);

                var userRoles = await _userManager.GetRolesAsync(user);
                var isAdmin = userRoles.Contains("Admin");

                var tokenInfo = await _jwtService.GenerateTokenAsync(user.Id);

                _logger.LogInformation("Login successful for user: {UserId}, roles: [{Roles}]",
                    user.Id, string.Join(", ", userRoles));

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Login successful",
                    Token = tokenInfo.AccessToken,
                    TokenInfo = tokenInfo,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email!,
                        Username = user.UserName!,
                        IsAdmin = isAdmin,
                        FullName = user.UserName ?? string.Empty,
                        CreatedAt = DateTime.UtcNow
                    }
                };
            }
            catch (AYYUAZ.APP.Application.Exceptions.AppException.AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for email: {Email}", loginDto?.Email ?? "null");
                throw new BadRequestException();
            }
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                _logger.LogInformation("Registration attempt started for email: {Email}", registerDto?.Email ?? "null");

                if (registerDto == null || string.IsNullOrEmpty(registerDto.Email) || string.IsNullOrEmpty(registerDto.Password))
                {
                    _logger.LogWarning("Registration failed - invalid input data");
                    throw new BadRequestException();
                }

                var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("Registration failed - user already exists for email: {Email}", registerDto.Email);
                    throw new BadRequestException();
                }

                var existingUsername = await _userManager.FindByNameAsync(registerDto.Username);
                if (existingUsername != null)
                {
                    _logger.LogWarning("Registration failed - username already exists: {Username}", registerDto.Username);
                    throw new BadRequestException();
                }

                var user = new User
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                    EmailConfirmed = false
                };

                _logger.LogDebug("Creating new user with email: {Email}, username: {UserName}",
                    registerDto.Email, user.UserName);

                var result = await _userManager.CreateAsync(user, registerDto.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Registration failed - user creation failed for email: {Email}, errors: {Errors}",
                        registerDto.Email, errors);

                    throw new BadRequestException();
                }

                await _userManager.AddToRoleAsync(user, "User");
                _logger.LogDebug("Assigned default role 'User' to user: {UserId}", user.Id);

                var userRoles = await _userManager.GetRolesAsync(user);
                var isAdmin = userRoles.Contains("Admin");

                var tokenInfo = await _jwtService.GenerateTokenAsync(user.Id);

                _logger.LogInformation("Registration successful for user: {UserId}, email: {Email}",
                    user.Id, user.Email);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Registration successful",
                    Token = tokenInfo.AccessToken,
                    TokenInfo = tokenInfo,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email!,
                        Username = user.UserName!,
                        IsAdmin = isAdmin,
                        FullName = user.UserName ?? string.Empty,
                        CreatedAt = DateTime.UtcNow
                    }
                };
            }
            catch (AYYUAZ.APP.Application.Exceptions.AppException.AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during registration for email: {Email}", registerDto?.Email ?? "null");
                throw new BadRequestException();
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                _logger.LogDebug("Token validation requested");

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Token validation failed - empty token");
                    return false;
                }

                var principal = _jwtService.ValidateToken(token);
                var isValid = principal != null;

                _logger.LogDebug("Token validation result: {IsValid}", isValid);
                return await Task.FromResult(isValid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token validation");
                return false;
            }
        }

        public async Task<ClaimsPrincipal?> GetPrincipalFromTokenAsync(string token)
        {
            try
            {
                _logger.LogDebug("Getting principal from token");

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Get principal failed - empty token");
                    return null;
                }

                var principal = _jwtService.ValidateToken(token);

                _logger.LogDebug("Principal extraction result: {HasPrincipal}", principal != null);
                return await Task.FromResult(principal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting principal from token");
                return null;
            }
        }

        public async Task<TokenDto> GenerateTokenAsync(string userId)
        {
            try
            {
                _logger.LogDebug("Token generation requested for user: {UserId}", userId);

                if (string.IsNullOrEmpty(userId))
                {
                    throw new BadRequestException();
                }

                var tokenInfo = await _jwtService.GenerateTokenAsync(userId);

                _logger.LogDebug("Token generated successfully for user: {UserId}", userId);
                return tokenInfo;
            }
            catch (AYYUAZ.APP.Application.Exceptions.AppException.AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating token for user: {UserId}", userId);
                throw new BadRequestException();
            }
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    throw new NotFoundException();

                var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Password changed successfully for user: {UserId}", userId);
                }
                else
                {
                    _logger.LogWarning("Password change failed for user: {UserId}, errors: {Errors}",
                        userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                    throw new BadRequestException();
                }

                return result.Succeeded;
            }
            catch (AYYUAZ.APP.Application.Exceptions.AppException.AppException)
            {
                throw;
            }
            catch
            {
                _logger.LogError("Error changing password for user: {UserId}", userId);
                throw new BadRequestException();
            }
        }

        public async Task<bool> ChangeEmailAsync(string userId, ChangeEmailDto changeEmailDto)
        {
            try
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
                    throw new UnauthorizedException();
                }

                var existingUser = await _userManager.FindByEmailAsync(changeEmailDto.NewEmail);
                if (existingUser != null && existingUser.Id != userId)
                {
                    _logger.LogWarning("Email change failed - email already in use: {NewEmail}", changeEmailDto.NewEmail);
                    throw new BadRequestException();
                }

                var token = await _userManager.GenerateChangeEmailTokenAsync(user, changeEmailDto.NewEmail);
                var result = await _userManager.ChangeEmailAsync(user, changeEmailDto.NewEmail, token);

                if (result.Succeeded)
                {
                    user.UserName = changeEmailDto.NewEmail;
                    var updateResult = await _userManager.UpdateAsync(user);
                    
                    if (updateResult.Succeeded)
                    {
                        _logger.LogInformation("Email changed successfully for user: {UserId}, new email: {NewEmail}", 
                            userId, changeEmailDto.NewEmail);
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("Email changed but username update failed for user: {UserId}, errors: {Errors}",
                            userId, string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                        return true;
                    }
                }
                else
                {
                    _logger.LogWarning("Email change failed for user: {UserId}, errors: {Errors}",
                        userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                    throw new BadRequestException();
                }
            }
            catch (AYYUAZ.APP.Application.Exceptions.AppException.AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing email for user: {UserId}", userId);
                throw new BadRequestException();
            }
        }
    }
}

