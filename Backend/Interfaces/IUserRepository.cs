using Backend.Common;
using Backend.DTOs;
using Backend.Entities;

namespace Backend.Interfaces;

public interface IUserRepository
{
  /// <summary>
  /// Retrieves a user by their unique ID.
  /// </summary>
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
  /// <param name="userParams">The username of the user to retrieve.</param>
  /// <returns>A paginated list of <see cref="MemberDto"/> objects.</returns>
  Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);

  /// <summary>
  /// Retrieves a user by their unique username.
  /// </summary>
  /// <param name="username">Demanded pagination data.</param>
  /// <returns><see cref="MemberDto"/> if found; otherwise, <c>null</c>.</returns>
  Task<MemberDto?> GetMemberAsync(string username);
}
