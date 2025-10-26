using Backend.Data;
using Backend.Data.Seed;
using Backend.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Extensions;

public static class HostExtensions
{
  public static async Task ApplyMigrationsAndSeedDatabase(this IHost host)
  {
    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
      var context = services.GetRequiredService<DataContext>();
      var userManager = services.GetRequiredService<UserManager<User>>();
      var roleManager = services.GetRequiredService<RoleManager<Role>>();
      await context.Database.MigrateAsync(); // apply pending migration to DB, create DB if it doesnt exist

      // seed mock data into tables
      await Seed.SeedUsers(userManager, roleManager);
      await Seed.SeedRoasters(context);
      await Seed.SeedBeans(context);
      await Seed.SeedBeanBatches(context);
      await Seed.SeedBrewMethods(context);
      await Seed.SeedBrewers(context);
      await Seed.SeedBrewerStockSettings(context);
      await Seed.SeedBrewerUserSettings(context);
      await Seed.SeedGrinders(context);
      await Seed.SeedGrinderDials(context);
      await Seed.SeedGrindingMechanisms(context);
      await Seed.SeedGrindingElements(context);
      await Seed.SeedGrinderElementCompatibilities(context);
      await Seed.SeedBrewSetups(context);
      await Seed.SeedUserBrewSetups(context);
      await Seed.SeedBrewSettings(context);
      await Seed.SeedBrewGrinderDialSettings(context);
    }
    catch (Exception ex)
    {
      var logger = services.GetRequiredService<ILogger<Program>>();
      logger.LogError(ex, "An error occurred during migration");
    }
  }
}
