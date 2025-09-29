using System.Security.Cryptography;
using System.Text;
using Backend.Data;
using Backend.DTOs;
using Backend.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

public class AccountController(DataContext context) : BaseApiController
{
  [HttpPost("register")] // account/register
  [AllowAnonymous]
  public async Task<ActionResult<User>> Register(RegisterDto registerDto)
  {
    if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

    using var hmac = new HMACSHA512(); // use using to tell it to dispose of this after out of scope

    var passwordByteArray = Encoding.UTF8.GetBytes(registerDto.Password);
    var user = new User
    {
      Username = registerDto.Username.ToLower(),
      PasswordHash = hmac.ComputeHash(passwordByteArray),
      PasswordSalt = hmac.Key
    };

    context.Users.Add(user);
    await context.SaveChangesAsync();

    return user;
  }

  private async Task<bool> UserExists(string username)
  {
    return await context.Users.AnyAsync(x => x.Username.ToLower() == username.ToLower());
  }
}
