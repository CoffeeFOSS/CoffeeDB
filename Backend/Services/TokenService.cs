using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Entities;
using Backend.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services;

public class TokenService(IConfiguration config) : ITokenService
{
  public string CreateToken(User user)
  {
    var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot access token key from appsettings");
    if (tokenKey.Length < 64) throw new Exception("TokenKey needs to be longer");

    var tokenKeyByteArray = Encoding.UTF8.GetBytes(tokenKey);
    var key = new SymmetricSecurityKey(tokenKeyByteArray);

    var claims = new List<Claim>
    {
      new(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new(ClaimTypes.Name, user.UserName)
    };

    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); // needs token length >= 64

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Expires = DateTime.UtcNow.AddDays(7), // TODO: need to strengthen security later
      SigningCredentials = credentials
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    var token = tokenHandler.CreateToken(tokenDescriptor);

    return tokenHandler.WriteToken(token);
  }
}
