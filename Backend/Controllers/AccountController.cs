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
}
