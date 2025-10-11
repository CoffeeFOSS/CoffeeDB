using Backend.DTOs;
using Backend.Interfaces;

namespace Backend.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
  public async Task<IEnumerable<MemberDto>> GetUsersAsync()
    => await userRepository.GetMembersAsync();

  public async Task<MemberDto> GetUserAsync(string username)
    => await userRepository.GetMemberAsync(username) ?? throw new Exception($"User {username} not found");
}
