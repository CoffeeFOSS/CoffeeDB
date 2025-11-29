using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.Entities;
using Backend.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class UserRepository(DataContext context) : IUserRepository
{
  public async Task<MemberDto?> GetMemberAsync(string username)
  {
    return await context.Users
      .Where(user => user.NormalizedUserName == username.ToUpperInvariant())
      .Select(user => new MemberDto
      {
        Username = user.UserName,
        Id = user.Id,
      })
      .SingleOrDefaultAsync();
  }

  public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
  {
    var query = context.Users
      .OrderByDescending(u => u.Id)
      .Select(user => new MemberDto
      {
        Username = user.UserName,
        Id = user.Id,
      });

    if (!string.IsNullOrWhiteSpace(userParams.Username))
    {
      var normalizedName = userParams.Username.ToLower();
      query = query
        .Where(u => u.Username != null && u.Username.ToLower().Contains(normalizedName));
    }

    return await PagedList<MemberDto>.CreateAsync(query, userParams.Page, userParams.PageSize);
  }

  public async Task<User?> GetUserByIdAsync(int id)
  {
    return await context.Users.FindAsync(id); // find via primary key, which is Id
  }

  public async Task<User?> GetUserByUsernameAsync(string username)
  {
    return await context.Users
      .SingleOrDefaultAsync(u => u.NormalizedUserName == username.ToUpperInvariant());
  }
}
