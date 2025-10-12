using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class RoleTestsController : BaseApiController
{
  [Authorize(Policy = "RequireAdminRole")]
  [HttpGet("users-with-admin-role")]
  public ActionResult GetUsersWithRoles()
  {
    return Ok("Only admins can see this");
  }
}
