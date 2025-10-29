using Backend.Common;
using Backend.DTOs;
using Backend.Extensions;
using Backend.Interfaces;

namespace Backend.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
  public async Task<PagedList<MemberDto>> GetUsersAsync(UserParams userParams, HttpResponse response)
  {
    var users = await userRepository.GetMembersAsync(userParams);
    response.AddPaginationHeader(users);

    return users;
  }

  public async Task<ServiceResult<MemberDto>> GetUserAsync(string username)
  {
    var user = await userRepository.GetMemberAsync(username);
    if (user == null) return ServiceResult<MemberDto>.Failure(404, $"User {username} not found");

    return ServiceResult<MemberDto>.Success(200, user);
  }
}
