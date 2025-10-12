using Backend.Common;
using Backend.DTOs;
using Backend.Entities;
using Backend.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class AdminService(UserManager<User> userManager) : IAdminService
{
  public async Task<List<UserWithRolesDto>> GetUsersWithRolesAsync()
  {
    var users = await userManager.Users
      .OrderBy(u => u.UserName)
      .Select(u => new UserWithRolesDto
      {
        Id = u.Id,
        Username = u.UserName,
        Roles = u.UserRoles.Select(r => r.Role.Name!).ToList()
      }).ToListAsync();

    return users;
  }

  public async Task<ServiceResult<List<string>>> EditRolesAsync(string username, string roles)
  {
    if (string.IsNullOrEmpty(roles)) return ServiceResult<List<string>>.Failure(400, "Must select at least one role");

    var selectedRoles = roles.Split(",").ToArray();

    var allowedRoles = new HashSet<string> { "Admin", "Moderator", "Member" };
    foreach (var role in selectedRoles)
    {
      if (!allowedRoles.Contains(role)) ServiceResult<List<string>>.Failure(400, $"Role {role} is not allowed");
    }

    var user = await userManager.FindByNameAsync(username);
    if (user == null) return ServiceResult<List<string>>.Failure(400, "User not found");

    var userRoles = await userManager.GetRolesAsync(user);

    var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
    if (!result.Succeeded) return ServiceResult<List<string>>.Failure(400, "Failed to add to roles");

    result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
    if (!result.Succeeded) return ServiceResult<List<string>>.Failure(400, "Failed to remove from roles");

    var updatedRoles = await userManager.GetRolesAsync(user);
    return ServiceResult<List<string>>.Success(200, [.. updatedRoles]);
  }
}
