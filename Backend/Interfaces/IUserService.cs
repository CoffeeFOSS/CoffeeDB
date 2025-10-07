using Backend.DTOs;

namespace Backend.Interfaces;

public interface IUserService
{
  /// <summary>
  /// Gets all users.
  /// </summary>
  /// <returns>A list of user information.</returns>
  Task<IEnumerable<MemberDto>> GetUsersAsync();

  /// <summary>
  /// Gets a user by their username.
  /// </summary>
  /// <param name="username">The username of the user.</param>
  /// <returns>The user information.</returns>
  Task<MemberDto> GetUserAsync(string username);
}
