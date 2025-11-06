using System.Security.Claims;
using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.Entities;
using Backend.Extensions;
using Backend.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace Backend.Services;

public class AdminService(UserManager<User> userManager) : IAdminService
{
  // TODO: much of this should be in a Repository layer
  public async Task<PagedList<UserWithRolesDto>> GetUsersWithRolesAsync(UserParams userParams, HttpResponse response)
  {
    var query = userManager.Users
      .OrderByDescending(u => u.Id)
      .Select(u => new UserWithRolesDto
      {
        Id = u.Id,
        Username = u.UserName,
        Roles = u.UserRoles.Select(r => r.Role.Name!).ToList()
      });

    if (!string.IsNullOrWhiteSpace(userParams.Username))
    {
      var normalizedName = userParams.Username.ToLower();
      query = query
        .Where(u => u.Username != null && u.Username.ToLower().Contains(normalizedName));
    }

    var users = await PagedList<UserWithRolesDto>.CreateAsync(query, userParams.Page, userParams.PageSize);
    response.AddPaginationHeader(users);

    return users;
  }

  public async Task<ServiceResult<List<string>>> EditRolesAsync(string username, string roles, ClaimsPrincipal currentUserClaims)
  {
    if (string.IsNullOrEmpty(roles)) return ServiceResult<List<string>>.Failure(400, "Must select at least one role");

    var selectedRoles = roles == "Empty" ? [] : roles.Split(",").ToArray();

    var allowedRoles = new HashSet<string> { "Admin", "Moderator" };
    if (allowedRoles.Count < selectedRoles.Length) return ServiceResult<List<string>>.Failure(400, "Too many roles were selected");

    foreach (var role in selectedRoles)
    {
      if (!allowedRoles.Contains(role)) return ServiceResult<List<string>>.Failure(400, $"Role {role} is not allowed");
    }

    var user = await userManager.FindByNameAsync(username);
    if (user == null) return ServiceResult<List<string>>.Failure(400, "User not found");

    var userRoles = await userManager.GetRolesAsync(user);

    var currentUser = await userManager.GetUserAsync(currentUserClaims);
    if (currentUser == null)
      return ServiceResult<List<string>>.Failure(401, "Unauthorized");

    if (currentUser.NormalizedUserName == username.ToUpperInvariant() && !selectedRoles.Contains("Admin"))
    {
      return ServiceResult<List<string>>.Failure(400, "Admins are not allowed to remove the admin role from themselves.");
    }

    var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
    if (!result.Succeeded) return ServiceResult<List<string>>.Failure(400, "Failed to add to roles");

    result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
    if (!result.Succeeded) return ServiceResult<List<string>>.Failure(400, "Failed to remove from roles");

    user.UpdatedById = currentUser.Id;
    user.UpdatedAt = DateTime.UtcNow;
    await userManager.UpdateAsync(user);

    var updatedRoles = await userManager.GetRolesAsync(user);
    return ServiceResult<List<string>>.Success(200, [.. updatedRoles]);
  }
}
