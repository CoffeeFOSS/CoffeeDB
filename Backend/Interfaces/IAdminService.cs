using System.Security.Claims;
using Backend.Common;
using Backend.DTOs;

namespace Backend.Interfaces;

public interface IAdminService
{
  /// <summary>
  /// Gets all users with roles
  /// </summary>
  /// <returns>List of <see cref="UserWithRolesDto"/> containing basic user info and their roles.</returns>
  // TODO: Consider using pagination to avoid return 1000000 users
  Task<PagedList<UserWithRolesDto>> GetUsersWithRolesAsync(UserParams userParams, HttpResponse response);

  /// <summary>
  /// Overwrites a user's roles
  /// </summary>
  /// <param name="username">The user's username.</param>
  /// <param name="roles">The new roles to set for the user.</param>
  /// <param name="currentUsername">The username of the authorized user that edits the roles.</param>
  /// <returns>List of roles.</returns>
  Task<ServiceResult<List<string>>> EditRolesAsync(string username, string roles, ClaimsPrincipal currentUserClaims);
}
