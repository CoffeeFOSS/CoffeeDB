using System.Security.Cryptography;
using System.Text;
using Backend.Common;
using Backend.Data;
using Backend.DTOs;
using Backend.Entities;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class AccountService(DataContext context, ITokenService tokenService) : IAccountService
{
  public async Task<ServiceResult<UserDto>> RegisterAsync(RegisterDto registerDto)
  {
    if (await UserExistsAsync(registerDto.Username))
    {
      return ServiceResult<UserDto>.Failure(400, "Username is already taken");
    }

    using var hmac = new HMACSHA512(); // use using to tell it to dispose of this after out of scope
    var passwordByteArray = Encoding.UTF8.GetBytes(registerDto.Password);
    var user = new User
    {
      UserName = registerDto.Username.ToLower(),
      PasswordHash = hmac.ComputeHash(passwordByteArray),
      PasswordSalt = hmac.Key
    };

    context.Users.Add(user);
    await context.SaveChangesAsync();

    var userDto = new UserDto
    {
      Username = user.UserName,
      Token = tokenService.CreateToken(user)
    };
    return ServiceResult<UserDto>.Success(201, userDto);
  }

  public async Task<ServiceResult<UserDto>> LoginAsync(LoginDto loginDto)
  {
    var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

    if (user == null)
    {
      return ServiceResult<UserDto>.Failure(401, "Invalid username or password");
    }

    using var hmac = new HMACSHA512(user.PasswordSalt);
    var passwordByteArray = Encoding.UTF8.GetBytes(loginDto.Password);
    var computedHash = hmac.ComputeHash(passwordByteArray);

    for (int i = 0; i < computedHash.Length; i++)
    {
      if (computedHash[i] != user.PasswordHash[i])
      {
        return ServiceResult<UserDto>.Failure(401, "Invalid username or password");
      }
    }

    var userDto = new UserDto
    {
      Username = user.UserName,
      Token = tokenService.CreateToken(user)
    };
    return ServiceResult<UserDto>.Success(200, userDto);
  }

  private async Task<bool> UserExistsAsync(string username)
  {
    return await context.Users.AnyAsync(x => x.UserName == username.ToLower());
  }
}
