using Backend.Common;
using Backend.DTOs;

namespace Backend.Interfaces;

public interface IAdminService
{
  /// <summary>
  /// Gets all users with roles
  /// </summary>
  /// <returns><see cref="UserWithRolesDto"/> containing basic user info and their roles.</returns>
  // TODO: Consider using pagination to avoid return 1000000 users
  Task<List<UserWithRolesDto>> GetUsersWithRolesAsync();

  /// <summary>
  /// Overwrites a user's roles
  /// </summary>
  /// <param name="username">The user's username.</param>
  /// <param name="roles">The new roles to set for the user.</param>
  /// <returns></returns>
  Task<ServiceResult<List<string>>> EditRolesAsync(string username, string roles);
}
