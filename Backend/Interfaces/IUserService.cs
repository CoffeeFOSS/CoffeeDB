using Backend.DTOs;

namespace Backend.Interfaces;

public interface IUserService
{
  // TODO: Summary
  Task<IEnumerable<MemberDto>> GetUsersAsync();
  Task<MemberDto> GetUserAsync(string username);
}
