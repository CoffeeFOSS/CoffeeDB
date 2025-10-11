using Backend.Common;
using Backend.DTOs;
using Backend.Entities;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class UserRepository(DataContext context) : IUserRepository
{
  public async Task<bool> SaveAllAsync()
  {
    return await context.SaveChangesAsync() > 0; // SaveChangesAsync returns # of changes saved in our DB 
  }

  public void Update(User user)
  {
    // Any update to an entity via the above will automatically let EF know user has been modified
    // This function lets EF know this user has been updated explicitly, if needed
    context.Entry(user).State = EntityState.Modified;
  }

  public async Task<MemberDto?> GetMemberAsync(string username)
  {
    return await context.Users
      .Where(user => user.UserName == username.ToLower())
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
      .Select(user => new MemberDto
      {
        Username = user.UserName,
        Id = user.Id,
      });

    return await PagedList<MemberDto>.CreateAsync(query, userParams.Page, userParams.PageSize);
  }

  public async Task<User?> GetUserByIdAsync(int id)
  {
    return await context.Users.FindAsync(id); // find via primary key, which is Id
  }

  public async Task<User?> GetUserByUsernameAsync(string username)
  {
    return await context.Users
      .SingleOrDefaultAsync(x => x.UserName == username.ToLower());
  }

  public async Task<IEnumerable<User>> GetUsersAsync()
  {
    return await context.Users
      .ToListAsync();
  }
}
