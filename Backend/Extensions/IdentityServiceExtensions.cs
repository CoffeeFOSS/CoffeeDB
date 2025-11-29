using System.Text;
using Backend.Data;
using Backend.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Extensions;

public static class IdentityServiceExtensions
{
  public static IServiceCollection AddIdentityServices(
    this IServiceCollection services,
    IConfiguration config
  )
  {
    services.AddIdentityCore<User>(opt =>
    {
      // By default, 
      // RequiredLength = 6
      // RequireDigit = true
      // RequireLowercase = true
      // RequireUppercase = true
      // RequireNonAlphanumeric = true
      opt.Password.RequireNonAlphanumeric = false;
    })
      // by default, AddIdentityCore adds userManager too
      .AddRoles<Role>()
      .AddRoleManager<RoleManager<Role>>()
      .AddEntityFrameworkStores<DataContext>();

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

    services.AddAuthorizationBuilder()
      // .AddPolicy("ModerateBrewRole", policy => policy.RequireRole("Admin", "Moderator")) // idk what to do with this yet, todo
      .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
      .AddPolicy("RequireModeratorRole", policy => policy.RequireRole("Moderator"));

    return services;
  }
}
