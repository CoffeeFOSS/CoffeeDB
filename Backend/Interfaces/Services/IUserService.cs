using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;

namespace Backend.Interfaces.Services;

public interface IUserService
{
  /// <summary>
  /// Gets all users.
  /// </summary>
  /// <param name="userParams">Pagination settings from user.</param>
  /// <param name="response">HttpResponse object from controller.</param>
  /// <returns>A paginated list of user information.</returns>
  Task<PagedList<MemberDto>> GetUsersAsync(UserParams userParams, HttpResponse response);

  /// <summary>
  /// Gets a user by their username.
  /// </summary>
  /// <param name="username">The username of the user.</param>
  /// <returns>The user information.</returns>
  Task<ServiceResult<MemberDto>> GetUserAsync(string username);
}
