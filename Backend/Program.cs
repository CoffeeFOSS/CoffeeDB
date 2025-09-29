using System.Text;
using Backend.Data;
using Backend.Interfaces;
using Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors();
builder.Services.AddScoped<ITokenService, TokenService>(); // create once per http request
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenKey = builder.Configuration["TokenKey"] ?? throw new Exception("TokenKey not found");
        var tokenKeyByteArray = Encoding.UTF8.GetBytes(tokenKey);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true, // must, otherwise it will accept any token, unsigned or not
            IssuerSigningKey = new SymmetricSecurityKey(tokenKeyByteArray),
            ValidateIssuer = false, // not passing in, so not needed for now
            ValidateAudience = false // not passing in, so not needed for now
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline (Middleware)
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

// app.UseHttpsRedirection();

app.MapControllers();

app.Run();
