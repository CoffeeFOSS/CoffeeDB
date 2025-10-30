using System.Security.Claims;
using Backend.Common;
using Backend.DTOs;
using Backend.Entities;
using Backend.Interfaces.Services;
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
      return ServiceResult<UserDto>.Failure(400, $"Username '{registerDto.Username}' is not available");
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
      return ServiceResult<UserDto>.Failure(400, result.Errors.First().Description);
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
    var user = await userManager.GetUserAsync(userClaims);
    if (user == null)
      return ServiceResult<UserDto>.Failure(401, "Unauthorized, user not found");

    if (user.UsernameUpdatedAt.HasValue)
    {
      var oneWeekAgo = DateTime.UtcNow.AddDays(-7);
      if (user.UsernameUpdatedAt.Value > oneWeekAgo)
      {
        var cooldown = user.UsernameUpdatedAt.Value.AddDays(7) - DateTime.UtcNow;
        return ServiceResult<UserDto>.Failure(400, $"Username can only be changed once per week. Cooldown: {cooldown.Days} days, {cooldown.Hours} hours, {cooldown.Minutes} minutes.");
      }
    }

    if (user.UserName == changeUsernameDto.NewUsername)
      return ServiceResult<UserDto>.Failure(400, "New username must be different from the current username.");

    if (!changeUsernameDto.NewUsername.All(c => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || (c == '-')))
      return ServiceResult<UserDto>.Failure(400, "New username can only contain alphanumeric or hyphen (-) characters");

    var existingUser = await userManager.Users.FirstOrDefaultAsync(u => u.UserName == changeUsernameDto.NewUsername);
    if (existingUser != null && existingUser.Id != user.Id)
      return ServiceResult<UserDto>.Failure(400, $"Username '{changeUsernameDto.NewUsername}' is not available.");

    if (!await userManager.CheckPasswordAsync(user, changeUsernameDto.Password))
      return ServiceResult<UserDto>.Failure(400, "Invalid password.");

    var result = await userManager.SetUserNameAsync(user, changeUsernameDto.NewUsername);
    if (!result.Succeeded)
      return ServiceResult<UserDto>.Failure(400, result.Errors.First().Description);

    user.UsernameUpdatedAt = DateTime.UtcNow;
    user.UpdatedById = user.Id;
    user.UpdatedAt = DateTime.UtcNow;
    await userManager.UpdateAsync(user);

    var userDto = new UserDto
    {
      Username = user.UserName!,
      Token = await tokenService.CreateToken(user)
    };
    return ServiceResult<UserDto>.Success(200, userDto);
  }

  public async Task<ServiceResult<UserDto>> ChangePasswordAsync(ChangePasswordDto changePasswordDto, ClaimsPrincipal userClaims)
  {
    var user = await userManager.GetUserAsync(userClaims);
    if (user == null)
      return ServiceResult<UserDto>.Failure(401, "Unauthorized, user not found");

    // not needed, this is already handled by ChangePasswordAsync,
    // but keep this before the other checks for more sensical error flow returned to client
    if (!await userManager.CheckPasswordAsync(user, changePasswordDto.CurrentPassword))
      return ServiceResult<UserDto>.Failure(400, "Invalid current password.");

    if (changePasswordDto.CurrentPassword == changePasswordDto.NewPassword)
      return ServiceResult<UserDto>.Failure(400, "New password must be different from current password.");

    if (changePasswordDto.ConfirmNewPassword != changePasswordDto.NewPassword)
      return ServiceResult<UserDto>.Failure(400, "Confirm new password must match new password.");

    var changePasswordResult = await userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
    if (!changePasswordResult.Succeeded)
    {
      var errors = string.Join(", ", changePasswordResult.Errors.Select(e => e.Description));
      return ServiceResult<UserDto>.Failure(400, $"Failed to change password: {errors}");
    }

    user.UpdatedById = user.Id;
    user.UpdatedAt = DateTime.UtcNow;
    await userManager.UpdateAsync(user);

    var userDto = new UserDto
    {
      Username = user.UserName!,
      Token = await tokenService.CreateToken(user)
    };
    return ServiceResult<UserDto>.Success(200, userDto);
  }
}
