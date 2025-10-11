namespace Backend.Data.Seed;

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Backend.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class Seed
{
  public static async Task SeedUsers(UserManager<User> userManager)
  {
    if (await userManager.Users.AnyAsync())
    {
      Console.WriteLine("there are users in the DB already");
      return;
    }

    var userData = await File.ReadAllTextAsync("Data/Seed/SeedData/UserSeedData.json");

    var users = JsonSerializer.Deserialize<List<User>>(userData, options);

    if (users == null) return;

    foreach (var user in users)
    {
      // ensure this pw satisfies requirements in IdentityServiceExtensions
      // otherwise seeding this user will fail, and you wont see an error
      await userManager.CreateAsync(user, "Pa$$w0rd");
    }
  }

  private static readonly JsonSerializerOptions options = new()
  {
    PropertyNameCaseInsensitive = true
  };
}
