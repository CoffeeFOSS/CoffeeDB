using Backend.Common;
using Backend.DTOs;
using Backend.Entities;

namespace Backend.Interfaces;

public interface IUserRepository
{
  /// <summary>
  /// Lets Entity Framework know this user has been updated explicitly.
  /// </summary>
  /// <param name="user">The user to update.</param>
  void Update(User user);

  /// <summary>
  /// Saves all changes to the database.
  /// </summary>
  /// <returns>True if the changes were saved successfully; otherwise, false.</returns>
  Task<bool> SaveAllAsync();


  /// <summary>
  /// Retrieves a user by their unique ID.
  /// /// </summary>
  /// <param name="id">The ID of the user to retrieve.</param>
  /// <returns><see cref="User"/> if found; otherwise, <c>null</c>.</returns>
  Task<User?> GetUserByIdAsync(int id);

  /// <summary> 
  /// Retrieves a user by their unique username.
  /// </summary>
  /// <param name="username">The username of the user to retrieve.</param>
  /// <returns><see cref="User"/> if found; otherwise, <c>null</c>.</returns>
  Task<User?> GetUserByUsernameAsync(string username);

  /// <summary>
  /// Retrieves a list of users.
  /// </summary>
  /// <returns>A paginated list of <see cref="MemberDto"/> objects.</returns>
  Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);

  /// <summary>
  /// Retrieves a user by their unique username.
  /// </summary>
  /// <param name="username">The username of the user to retrieve.</param>
  /// <returns><see cref="MemberDto"/> if found; otherwise, <c>null</c>.</returns>
  Task<MemberDto?> GetMemberAsync(string username);
}
