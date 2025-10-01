using Backend.Entities;

namespace Backend.Interfaces;

public interface ITokenService
{
  string CreateToken(User user);
  
}
