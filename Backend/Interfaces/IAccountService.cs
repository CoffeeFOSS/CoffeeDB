using Backend.Common;
using Backend.DTOs;

namespace Backend.Interfaces;

public interface IAccountService
{
  // TODO: Summary
  Task<ServiceResult<UserDto>> RegisterAsync(RegisterDto registerDto);
  Task<ServiceResult<UserDto>> LoginAsync(LoginDto loginDto);
}
