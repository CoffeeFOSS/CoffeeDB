namespace Backend.Data.Seed;

using System.Text.Json;
using Backend.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class Seed
{
  public static async Task SeedUsers(UserManager<User> userManager, RoleManager<Role> roleManager)
  {
    if (await userManager.Users.AnyAsync())
    {
      Console.WriteLine("there are users in the DB already");
      return;
    }

    var userData = await File.ReadAllTextAsync("Data/Seed/SeedData/UserSeedData.json");

    var users = JsonSerializer.Deserialize<List<User>>(userData, options);

    if (users == null) return;

    var roles = new List<Role>
    {
      new() {Name = "Moderator"},
      new() {Name = "Admin"}
    };

    foreach (var role in roles)
    {
      await roleManager.CreateAsync(role);
    }

    foreach (var user in users)
    {
      if (user.UserName == null)
      {
        Console.WriteLine("Seed data is missing username", user);
        continue;
      }
      user.UserName = user.UserName.ToLower();

      // ensure this pw satisfies requirements in IdentityServiceExtensions
      // otherwise seeding this user will fail, and you wont see an error
      await userManager.CreateAsync(user, "Pa$$w0rd");
    }

    var admin = new User { UserName = "admin" };
    await userManager.CreateAsync(admin, "Pa$$w0rd");
    await userManager.AddToRolesAsync(admin, ["Admin", "Moderator"]); // is all 3 needed
  }

  private static readonly JsonSerializerOptions options = new()
  {
    PropertyNameCaseInsensitive = true
  };
}
