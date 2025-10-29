using Backend.Data;
using Backend.Interfaces;
using Backend.Repository;
using Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Backend.Extensions;

// static allows us to use the methods inside this class without creating a new instance
public static class ApplicationServiceExtensions
{
  public static IServiceCollection AddApplicationServices(
    this IServiceCollection services,
    IConfiguration config
  )
  {
    services.AddControllers();

    // Database
    services.AddDbContext<DataContext>(opt =>
      { opt.UseNpgsql(config.GetConnectionString("DefaultConnection")).UseSnakeCaseNamingConvention(); });

    // Cross-Origin Resource Sharing
    services.AddCors();

    // Dependency Injection Registration
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IRoasterRepository, RoasterRepository>();

    services.AddScoped<ITokenService, TokenService>(); // create once per http request
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IAccountService, AccountService>();
    services.AddScoped<IAdminService, AdminService>();
    services.AddScoped<IRoasterService, RoasterService>();

    // API Documentation
    services.AddSwaggerGen(c =>
    {
      c.SwaggerDoc("v1", new OpenApiInfo
      {
        Title = "CoffeeDB API",
        Version = "v1"
      });
      c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
      {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer <token>' in the input below.\r\n\r\nExample: \"Bearer fulltokenhere\"",
      });
      c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    });

    return services;
  }
}
