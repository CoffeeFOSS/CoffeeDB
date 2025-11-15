using Backend.Data;
using Backend.Data.Seed;
using Backend.Entities;
using Backend.Entities.Revision;
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
      await Seed.SeedTable<EntityRevision>(context, "EntityRevisions.json");
      await Seed.SeedUsers(userManager, roleManager);
      await Seed.SeedRoasters(context, "Roasters.json");
      await Seed.SeedRoasterRevisions(context, "RoasterRevisions.json");
      await Seed.SeedTable<Brand>(context, "Brands.json");
      await Seed.SeedTable<Bean>(context, "Beans.json");
      await Seed.SeedTable<BeanBatch>(context, "BeanBatches.json");
      await Seed.SeedTable<BrewMethod>(context, "BrewMethods.json");
      await Seed.SeedTable<Brewer>(context, "Brewers.json");
      await Seed.SeedTable<BrewerStockSetting>(context, "BrewerStockSettings.json");
      await Seed.SeedTable<BrewerUserSetting>(context, "BrewerUserSettings.json");
      await Seed.SeedTable<Grinder>(context, "Grinders.json");
      await Seed.SeedTable<GrinderDial>(context, "GrinderDials.json");
      await Seed.SeedTable<GrindingMechanism>(context, "GrindingMechanisms.json");
      await Seed.SeedTable<GrindingElement>(context, "GrindingElements.json");
      await Seed.SeedTable<GrinderElementCompatibility>(context, "GrinderElementCompatibilities.json");
      await Seed.SeedTable<BrewSetup>(context, "BrewSetups.json");
      await Seed.SeedTable<UserBrewSetup>(context, "UserBrewSetups.json");
      await Seed.SeedTable<BrewSetting>(context, "BrewSettings.json");
      await Seed.SeedTable<BrewGrinderDialSetting>(context, "BrewGrinderDialSettings.json");
    }
    catch (Exception ex)
    {
      var logger = services.GetRequiredService<ILogger<Program>>();
      logger.LogError(ex, "An error occurred during migration");
    }
  }
}
