using Backend.Common;
using Backend.Extensions;
using Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class AdminController(IAdminService adminService) : BaseApiController
{
  [Authorize(Policy = "RequireAdminRole")]
  [HttpGet("users-with-roles")]
  [ProducesResponseType(200)]
  [ProducesResponseType(401)]
  public async Task<IActionResult> GetUsersWithRoles([FromQuery] UserParams userParams)
    => Ok(await adminService.GetUsersWithRolesAsync(userParams, Response));

  [Authorize(Policy = "RequireAdminRole")]
  [HttpPost("edit-roles/{username}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(401)]
  public async Task<IActionResult> EditRoles(string username, string roles)
    => (await adminService.EditRolesAsync(username, roles, User.Identity?.Name!)).ToActionResult();
}
