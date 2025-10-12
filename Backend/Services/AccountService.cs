using Backend.Common;
using Backend.DTOs;
using Backend.Entities;
using Backend.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class AccountService(UserManager<User> userManager, ITokenService tokenService) : IAccountService
{
  public async Task<ServiceResult<UserDto>> RegisterAsync(RegisterDto registerDto)
  {
    if (string.IsNullOrWhiteSpace(registerDto.Username) || string.IsNullOrWhiteSpace(registerDto.Password))
    {
      return ServiceResult<UserDto>.Failure(400, "Username and password must be provided");
    }
    if (await UserExistsAsync(registerDto.Username))
    {
      return ServiceResult<UserDto>.Failure(400, "Username is already taken");
    }
    if (registerDto.Password != registerDto.ConfirmPassword)
    {
      return ServiceResult<UserDto>.Failure(400, "Password and Confirm Password must match");
    }

    var user = new User
    {
      UserName = registerDto.Username.ToLower(), // for consistency let's just make usernames lowercase
    };

    var result = await userManager.CreateAsync(user, registerDto.Password);

    if (!result.Succeeded)
    {
      var errorString = string.Join(" | ", result.Errors.Select(e => e.Description));
      return ServiceResult<UserDto>.Failure(400, errorString);
    }

    var userDto = new UserDto
    {
      Username = user.UserName,
      Token = tokenService.CreateToken(user)
    };
    return ServiceResult<UserDto>.Success(201, userDto);
  }

  public async Task<ServiceResult<UserDto>> LoginAsync(LoginDto loginDto)
  {
    if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
    {
      return ServiceResult<UserDto>.Failure(400, "Username and password must be provided");
    }

    var user = await userManager.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == loginDto.Username.ToUpper());

    if (user == null || user.UserName == null)
    {
      return ServiceResult<UserDto>.Failure(401, "Invalid username or password");
    }

    var result = await userManager.CheckPasswordAsync(user, loginDto.Password);

    if (!result)
    {
      return ServiceResult<UserDto>.Failure(401, "Invalid username or password");
    }

    // return ServiceResult<UserDto>.Failure(401, "Invalid username or password");

    var userDto = new UserDto
    {
      Username = user.UserName,
      Token = tokenService.CreateToken(user)
    };
    return ServiceResult<UserDto>.Success(200, userDto);
  }

  private async Task<bool> UserExistsAsync(string username)
  {
    return await userManager.Users.AnyAsync(u => u.NormalizedUserName == username.ToUpper());
  }
}
