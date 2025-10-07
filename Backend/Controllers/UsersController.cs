using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class UsersController(IUserService userService) : BaseApiController
{
  [AllowAnonymous]
  [HttpGet]
  [ProducesResponseType(200)]
  public async Task<IActionResult> GetUsers()
    => Ok(await userService.GetUsersAsync());

  [AllowAnonymous]
  [HttpGet("{username}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> GetUser(string username)
    => Ok(await userService.GetUserAsync(username));
}
