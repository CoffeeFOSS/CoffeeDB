using Backend.Common;
using Backend.Entities;
using Backend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

// TODO: Refactor to AdminService
public class AdminController(UserManager<User> userManager) : BaseApiController
{
  [Authorize(Policy = "RequireAdminRole")]
  [HttpGet("users-with-roles")]
  public async Task<IActionResult> GetUsersWithRoles()
  {
    var users = await userManager.Users
      .OrderBy(u => u.UserName)
      .Select(u => new
      {
        u.Id,
        Username = u.UserName,
        Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
      }).ToListAsync();

    return Ok(users);
  }

  [Authorize(Policy = "RequireAdminRole")]
  [HttpPost("edit-roles/{username}")]
  public async Task<IActionResult> EditRoles(string username, string roles)
  {
    if (string.IsNullOrEmpty(roles)) return ServiceResult<List<string>>.Failure(400, "Must select at least one role").ToActionResult();

    var selectedRoles = roles.Split(",").ToArray();

    var user = await userManager.FindByNameAsync(username);
    if (user == null) return ServiceResult<List<string>>.Failure(400, "User not found").ToActionResult();

    var userRoles = await userManager.GetRolesAsync(user);

    var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
    if (!result.Succeeded) return ServiceResult<List<string>>.Failure(400, "Failed to add to roles").ToActionResult();

    result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
    if (!result.Succeeded) return ServiceResult<List<string>>.Failure(400, "Failed to remove from roles").ToActionResult();

    return Ok(await userManager.GetRolesAsync(user));
  }
}
