using Backend.Common;
using Backend.DTOs;

namespace Backend.Interfaces;

public interface IUserService
{
  /// <summary>
  /// Gets all users.
  /// </summary>
  /// <param name="userParams">Pagination settings from user.</params>
  /// <param name="response">HttpResponse object from controller.</params>
  /// <returns>A <see cref="PagedList"/> of user information.</returns>
  Task<PagedList<MemberDto>> GetUsersAsync(UserParams userParams, HttpResponse response);

  /// <summary>
  /// Gets a user by their username.
  /// </summary>
  /// <param name="username">The username of the user.</param>
  /// <returns>The user information.</returns>
  Task<MemberDto> GetUserAsync(string username);
}
