using Backend.Entities;

namespace Backend.Interfaces.Services;

public interface ITokenService
{
  /// <summary>
  /// Creates a JSON Web Token (JWT) for the specified user.
  /// </summary>
  /// <param name="user">The user for whom to create the token.</param>
  /// <returns>A JWT as a string.</returns>
  Task<string> CreateToken(User user);
}
