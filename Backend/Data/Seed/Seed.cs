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

      // ensure this pw satisfies requirements in IdentityServiceExtensions
      // otherwise seeding this user will fail, and you wont see an error
      await userManager.CreateAsync(user, "Pa$$w0rd");
    }

    var admin = new User { UserName = "admin" };
    await userManager.CreateAsync(admin, "Pa$$w0rd");
    await userManager.AddToRolesAsync(admin, ["Admin", "Moderator"]); // is all 3 needed
  }

  public static async Task SeedRoasters(DataContext context)
  {
    if (await context.Roasters.AnyAsync())
    {
      Console.WriteLine("Roasters is not an empty table");
      return;
    }
    // TODO
    throw new NotImplementedException();
  }

  public static async Task SeedBeans(DataContext context)
  {
    if (await context.Beans.AnyAsync())
    {
      Console.WriteLine("Beans is not an empty table");
      return;
    }
    // TODO
    throw new NotImplementedException();
  }

  public static async Task SeedBeanBatches(DataContext context)
  {
    if (await context.BeanBatches.AnyAsync())
    {
      Console.WriteLine("BeanBatches is not an empty table");
      return;
    }
    // TODO
    throw new NotImplementedException();
  }

  public static async Task SeedBrewMethods(DataContext context)
  {
    if (await context.BrewMethods.AnyAsync())
    {
      Console.WriteLine("BrewMethods is not an empty table");
      return;
    }
    // TODO
    throw new NotImplementedException();
  }

  public static async Task SeedBrewers(DataContext context)
  {
    if (await context.Brewers.AnyAsync())
    {
      Console.WriteLine("Brewers is not an empty table");
      return;
    }
    // TODO
    throw new NotImplementedException();
  }

  public static async Task SeedBrewerStockSettings(DataContext context)
  {
    if (await context.BrewerStockSettings.AnyAsync())
    {
      Console.WriteLine("BrewerStockSettings is not an empty table");
      return;
    }
    // TODO
    throw new NotImplementedException();
  }

  public static async Task SeedBrewerUserSettings(DataContext context)
  {
    if (await context.BrewerUserSettings.AnyAsync())
    {
      Console.WriteLine("BrewerUserSettings is not an empty table");
      return;
    }
    // TODO
    throw new NotImplementedException();
  }

  public static async Task SeedGrinders(DataContext context)
  {
    if (await context.Grinders.AnyAsync())
    {
      Console.WriteLine("Grinders is not an empty table");
      return;
    }
    // TODO
    throw new NotImplementedException();
  }

  public static async Task SeedGrinderDials(DataContext context)
  {
    if (await context.GrinderDials.AnyAsync())
    {
      Console.WriteLine("GrinderDials is not an empty table");
      return;
    }
    // TODO
    throw new NotImplementedException();
  }

  public static async Task SeedGrindingMechanisms(DataContext context)
  {
    if (await context.GrindingMechanisms.AnyAsync())
    {
      Console.WriteLine("GrindingMechanisms is not an empty table");
      return;
    }
    // TODO
    throw new NotImplementedException();
  }

  public static async Task SeedGrindingElements(DataContext context)
  {
    if (await context.GrindingElements.AnyAsync())
    {
      Console.WriteLine("GrindingElements is not an empty table");
      return;
    }
    // TODO
    throw new NotImplementedException();
  }

  public static async Task SeedGrinderElementCompatibilities(DataContext context)
  {
    if (await context.GrinderElementCompatibilities.AnyAsync())
    {
      Console.WriteLine("GrinderElementCompatibilities is not an empty table");
      return;
    }
    // TODO
    throw new NotImplementedException();
  }

  public static async Task SeedBrewSetups(DataContext context)
  {
    if (await context.BrewSetups.AnyAsync())
    {
      Console.WriteLine("BrewSetups is not an empty table");
      return;
    }
    // TODO
    throw new NotImplementedException();
  }

  public static async Task SeedUserBrewSetups(DataContext context)
  {
    if (await context.UserBrewSetups.AnyAsync())
    {
      Console.WriteLine("UserBrewSetups is not an empty table");
      return;
    }
    // TODO
    throw new NotImplementedException();
  }

  public static async Task SeedBrewSettings(DataContext context)
  {
    if (await context.BrewSettings.AnyAsync())
    {
      Console.WriteLine("BrewSettings is not an empty table");
      return;
    }
    // TODO
    throw new NotImplementedException();
  }

  public static async Task SeedBrewGrinderDialSettings(DataContext context)
  {
    if (await context.BrewGrinderDialSettings.AnyAsync())
    {
      Console.WriteLine("BrewGrinderDialSettings is not an empty table");
      return;
    }
    // TODO
    throw new NotImplementedException();
  }

  private static readonly JsonSerializerOptions options = new()
  {
    PropertyNameCaseInsensitive = true
  };
}
