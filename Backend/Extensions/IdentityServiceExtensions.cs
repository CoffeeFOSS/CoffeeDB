using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Extensions;

public static class IdentityServiceExtensions
{
  public static IServiceCollection AddIdentityServices(
    this IServiceCollection services,
    IConfiguration config
  )
  {
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      var tokenKey = config["TokenKey"] ?? throw new Exception("TokenKey not found");
      var tokenKeyByteArray = Encoding.UTF8.GetBytes(tokenKey);

      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true, // must, otherwise it will accept any token, unsigned or not
        IssuerSigningKey = new SymmetricSecurityKey(tokenKeyByteArray),
        ValidateIssuer = false, // not passing in, so not needed for now
        ValidateAudience = false // not passing in, so not needed for now
      };
    });

    return services;
  }
}
