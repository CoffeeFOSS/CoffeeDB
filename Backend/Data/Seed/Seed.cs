namespace Backend.Data.Seed;

using System.Text.Json;
using Backend.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class Seed
{
  private const string SeedDataDir = "Data/Seed/SeedData";

  public static async Task SeedUsers(UserManager<User> userManager, RoleManager<Role> roleManager)
  {
    if (await userManager.Users.AnyAsync())
    {
      Console.WriteLine("there are users in the DB already");
      return;
    }

    var seedPath = Path.Combine(SeedDataDir, "Users.json");
    var seedData = await File.ReadAllTextAsync(seedPath);
    var users = JsonSerializer.Deserialize<List<User>>(seedData, options);
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

      // ensure this pw satisfies requirements in IdentityServiceExtensions
      // otherwise seeding this user will fail, and you wont see an error
      await userManager.CreateAsync(user, "Pa$$w0rd");
    }

    var admin = new User { UserName = "admin" };
    await userManager.CreateAsync(admin, "Pa$$w0rd");
    await userManager.AddToRolesAsync(admin, ["Admin", "Moderator"]); // is all 3 needed
  }

  public static async Task SeedTable<T>(DataContext context, string seedFileName) where T : class
  {
    var entityType = context.Model.FindEntityType(typeof(T));
    if (entityType == null)
    {
      Console.WriteLine($"Error: Type {typeof(T).Name} is not a recognized entity in the DataContext model. Skipping seed.");
      return;
    }

    var dbSet = context.Set<T>();
    if (await dbSet.AnyAsync())
    {
      Console.WriteLine($"{typeof(T).Name} is not an empty table. Skipping seed.");
      return;
    }

    var seedPath = Path.Combine(SeedDataDir, seedFileName);

    if (!File.Exists(seedPath))
    {
      Console.WriteLine($"Error: Seed file not found at {seedPath}. Skipping seed.");
      return;
    }

    var seedData = await File.ReadAllTextAsync(seedPath);
    var data = JsonSerializer.Deserialize<List<T>>(seedData);
    if (data == null)
    {
      Console.WriteLine($"Warning: Seed file {seedFileName} contained no valid data. Skipping seed.");
      return;
    }

    await dbSet.AddRangeAsync(data);
    await context.SaveChangesAsync();
    Console.WriteLine($"Successfully seeded {typeof(T).Name} data.");
  }

  private static readonly JsonSerializerOptions options = new()
  {
    PropertyNameCaseInsensitive = true
  };
}
