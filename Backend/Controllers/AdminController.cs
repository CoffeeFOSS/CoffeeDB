using Backend.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

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
}
