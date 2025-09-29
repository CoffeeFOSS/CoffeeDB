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

  [HttpPost("login")]
  public async Task<ActionResult<User>> Login(LoginDto loginDto)
  {
    var user = await context.Users.FirstOrDefaultAsync(x => x.Username == loginDto.Username.ToLower());

    if (user == null) return Unauthorized("Invalid username or password");

    using var hmac = new HMACSHA512(user.PasswordSalt);

    var passwordByteArray = Encoding.UTF8.GetBytes(loginDto.Password);

    var computedHash = hmac.ComputeHash(passwordByteArray);

    for (int i = 0; i < computedHash.Length; i++)
    {
      if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
    }

    return user;
  }

  private async Task<bool> UserExists(string username)
  {
    return await context.Users.AnyAsync(x => x.Username == username.ToLower());
  }
}
