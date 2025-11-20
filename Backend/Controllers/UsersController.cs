using Backend.Common.Params;
using Backend.Extensions;
using Backend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class UsersController(IUserService userService) : BaseApiController
{
  [AllowAnonymous]
  [HttpGet]
  [ProducesResponseType(200)]
  public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
    => Ok(await userService.GetUsersAsync(userParams, Response));

  [AllowAnonymous]
  [HttpGet("{username}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> GetUser(string username)
    => (await userService.GetUserAsync(username)).ToActionResult();
}
