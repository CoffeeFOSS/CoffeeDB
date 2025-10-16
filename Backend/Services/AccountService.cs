using System.Security.Claims;
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
    if (!registerDto.Username.All(c => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || (c == '-')))
    {
      return ServiceResult<UserDto>.Failure(400, "Username can only contain alphanumeric or hyphen (-) characters");
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
      UserName = registerDto.Username,
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
      Token = await tokenService.CreateToken(user)
    };
    return ServiceResult<UserDto>.Success(201, userDto);
  }

  public async Task<ServiceResult<UserDto>> LoginAsync(LoginDto loginDto)
  {
    if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
    {
      return ServiceResult<UserDto>.Failure(400, "Username and password must be provided");
    }

    var user = await userManager.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == loginDto.Username.ToUpperInvariant());

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
      Token = await tokenService.CreateToken(user)
    };
    return ServiceResult<UserDto>.Success(200, userDto);
  }

  private async Task<bool> UserExistsAsync(string username)
  {
    return await userManager.Users.AnyAsync(u => u.NormalizedUserName == username.ToUpperInvariant());
  }

  public async Task<ServiceResult<UserDto>> ChangeUsernameAsync(ChangeUsernameDto changeUsernameDto, ClaimsPrincipal userClaims)
  {
    // Get the user via claims
    var user = await userManager.GetUserAsync(userClaims);

    if (user == null)
    {
      return ServiceResult<UserDto>.Failure(401, "Unauthorized, user not found");
    }

    // Check if the new username is the same as the current username (should fail)
    if (user.NormalizedUserName == changeUsernameDto.NewUsername.ToUpperInvariant())
    {
      return ServiceResult<UserDto>.Failure(400, "New username must be different from the current username.");
    }

    // Check if the new username is not either alphanumeric or hyphens (-)
    if (!changeUsernameDto.NewUsername.All(c => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || (c == '-')))
    {
      return ServiceResult<UserDto>.Failure(400, "New username can only contain alphanumeric or hyphen (-) characters");
    }

    // Check if the username is already taken by someone else (should fail)
    if (await userManager.FindByNameAsync(changeUsernameDto.NewUsername) != null)
    {
      return ServiceResult<UserDto>.Failure(400, $"Username '{changeUsernameDto.NewUsername}' is not available.");
    }

    // Check if the password is correct 
    if (!await userManager.CheckPasswordAsync(user, changeUsernameDto.Password))
    {
      return ServiceResult<UserDto>.Failure(400, "Invalid password.");
    }

    // Check if the username has been changed within the last week (use LastUsernameUpdatedAt)
    if (user.UsernameUpdatedAt.HasValue)
    {
      var oneWeekAgo = DateTime.UtcNow.AddDays(-7);
      if (user.UsernameUpdatedAt.Value > oneWeekAgo)
      {
        var cooldown = user.UsernameUpdatedAt.Value.AddDays(7) - DateTime.UtcNow;
        return ServiceResult<UserDto>.Failure(400, $"Username can only be changed once per week. Cooldown: {cooldown.Days} days, {cooldown.Hours} hours, {cooldown.Minutes} minutes.");
      }
    }

    // Change the username
    user.UserName = changeUsernameDto.NewUsername;
    user.NormalizedUserName = changeUsernameDto.NewUsername.ToUpperInvariant();
    user.UsernameUpdatedAt = now;

    // Update the LastUsernameUpdatedAt
    throw new NotImplementedException();
  }

  public Task<ServiceResult<UserDto>> ChangePasswordAsync(ChangePasswordDto changePasswordDto, ClaimsPrincipal userClaims)
  {
    throw new NotImplementedException();
  }
}
