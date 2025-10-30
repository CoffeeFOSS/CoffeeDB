using System.Security.Claims;
using Backend.Common;
using Backend.DTOs;

namespace Backend.Interfaces.Services;

public interface IAccountService
{
  /// <summary>
  /// Registers a new user.
  /// </summary>
  /// <param name="registerDto">The registration details.</param>
  /// <returns><see cref="UserDto"/> Registered user information and token.</returns>
  Task<ServiceResult<UserDto>> RegisterAsync(RegisterDto registerDto);

  /// <summary>
  /// Logs in an existing user.
  /// </summary>
  /// <param name="loginDto">The login credentials.</param>
  /// <returns><see cref="UserDto"/> Logged in user information and token.</returns>
  Task<ServiceResult<UserDto>> LoginAsync(LoginDto loginDto);

  /// <summary>
  /// Changes the logged in user's username.
  /// </summary>
  /// <param name="changeUsernameDto">Credentials with new username.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns><see cref="UserDto"/> Updated user information and token.</returns>
  Task<ServiceResult<UserDto>> ChangeUsernameAsync(ChangeUsernameDto changeUsernameDto, ClaimsPrincipal userClaims);

  /// <summary>
  /// Changes the logged in user's password.
  /// </summary>
  /// <param name="changePasswordDto">Credentials with new password.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns><see cref="UserDto"/> Updated user information and token.</returns>
  Task<ServiceResult<UserDto>> ChangePasswordAsync(ChangePasswordDto changePasswordDto, ClaimsPrincipal userClaims);
}
