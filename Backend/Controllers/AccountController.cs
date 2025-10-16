using Backend.DTOs;
using Backend.Extensions;
using Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class AccountController(IAccountService accountService) : BaseApiController
{
  [HttpPost("register")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  public async Task<IActionResult> Register(RegisterDto registerDto)
    => (await accountService.RegisterAsync(registerDto)).ToActionResult();

  [HttpPost("login")]
  [ProducesResponseType(200)]
  [ProducesResponseType(401)]
  public async Task<IActionResult> Login(LoginDto loginDto)
    => (await accountService.LoginAsync(loginDto)).ToActionResult();

  [Authorize]
  [HttpGet("checkAuth")]
  [ProducesResponseType(200)]
  [ProducesResponseType(401)]
  public IActionResult CheckAuth() => Ok();

  [Authorize]
  [HttpPost("change-username")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(401)]
  // TODO: ProducesResponseTypes
  public async Task<IActionResult> ChangePassword(ChangeUsernameDto changeUsernameDto)
    => (await accountService.ChangeUsernameAsync(changeUsernameDto)).ToActionResult();

  [Authorize]
  [HttpPost("change-password")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(401)]
  // TODO: ProducesResponseTypes
  public async Task<IActionResult> ChangeUsername(ChangePasswordDto changePasswordDto)
    => (await accountService.ChangePasswordAsync(changePasswordDto)).ToActionResult();

}
