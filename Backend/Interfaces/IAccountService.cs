using Backend.Common;
using Backend.DTOs;

namespace Backend.Interfaces;

public interface IAccountService
{
  /// <summary>
  /// Registers a new user.
  /// </summary>
  /// <param name="registerDto">The registration details.</param>
  /// <returns><see cref="UserDto"/> Registered user information.</returns>
  Task<ServiceResult<UserDto>> RegisterAsync(RegisterDto registerDto);

  /// <summary>
  /// Logs in an existing user.
  /// </summary>
  /// <param name="loginDto">The login credentials.</param>
  /// <returns><see cref="UserDto"/> Logged in user information.</returns>
  Task<ServiceResult<UserDto>> LoginAsync(LoginDto loginDto);
}
