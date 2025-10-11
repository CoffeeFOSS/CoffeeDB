using Backend.Common;
using Backend.DTOs;
using Backend.Extensions;
using Backend.Interfaces;

namespace Backend.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
  public async Task<IEnumerable<MemberDto>> GetUsersAsync(UserParams userParams, HttpResponse response)
  {
    var users = await userRepository.GetMembersAsync(userParams);
    response.AddPaginationHeader(users);

    return users;
  }

  public async Task<MemberDto> GetUserAsync(string username)
    => await userRepository.GetMemberAsync(username) ?? throw new Exception($"User {username} not found");
}
