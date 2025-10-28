using Backend.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Backend.Data;

public class DataContext(DbContextOptions options) : IdentityDbContext<
  // order matters here
  User,
  Role,
  int,
  IdentityUserClaim<int>,
  UserRole,
  IdentityUserLogin<int>,
  IdentityRoleClaim<int>,
  IdentityUserToken<int>
>(options)
{
  public DbSet<Brand> Brands { get; set; } // Equipment Brands
  public DbSet<Roaster> Roasters { get; set; }
  public DbSet<Bean> Beans { get; set; }
  public DbSet<BrewMethod> BrewMethods { get; set; }
  public DbSet<Brewer> Brewers { get; set; }
  public DbSet<BeanBatch> BeanBatches { get; set; }
  public DbSet<Grinder> Grinders { get; set; }
  public DbSet<GrindingElement> GrindingElements { get; set; } // Burrs, Blades
  public DbSet<GrindingMechanism> GrindingMechanisms { get; set; } // Flat Burr, Conical Burr, Blade
  public DbSet<GrinderElementCompatibility> GrinderElementCompatibilities { get; set; } // 54mm Flat Burr used by DF54
  public DbSet<BrewSetup> BrewSetups { get; set; }
  public DbSet<UserBrewSetup> UserBrewSetups { get; set; }
  public DbSet<GrinderDial> GrinderDials { get; set; }
  public DbSet<BrewGrinderDialSetting> BrewGrinderDialSettings { get; set; }
  public DbSet<BrewSetting> BrewSettings { get; set; }
  public DbSet<BrewerStockSetting> BrewerStockSettings { get; set; }
  public DbSet<BrewerUserSetting> BrewerUserSettings { get; set; }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    /***** Setup Relations on Entities with Audit Data *****/
    ConfigureAuditableEntity(builder.Entity<BeanBatch>());
    ConfigureAuditableEntity(builder.Entity<UserBrewSetup>());
    ConfigureAuditableEntity(builder.Entity<BrewSetting>());
    ConfigureAuditableEntity(builder.Entity<BrewerUserSetting>());
    ConfigureAuditableEntity(builder.Entity<BrewGrinderDialSetting>());
    ConfigureAuditableEntity(builder.Entity<BrewerUserSetting>());

    /***** Setup Relations on Entities *****/
    builder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
  }

  private static void ConfigureAuditableEntity<TEntity>(EntityTypeBuilder<TEntity> builder) where TEntity : class, IAuditable
  {
    builder.HasOne(ae => ae.CreatedBy)
        .WithMany()
        .HasForeignKey(ae => ae.CreatedById)
        .OnDelete(DeleteBehavior.SetNull);

    builder.HasOne(ae => ae.UpdatedBy)
        .WithMany()
        .HasForeignKey(ae => ae.UpdatedById)
        .OnDelete(DeleteBehavior.SetNull);
  }
}
