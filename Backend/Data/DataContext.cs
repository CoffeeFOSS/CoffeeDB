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

    /*
    > Many to One
    < One to Many
    <-> Many to Many
    */

    /***** Setup Relations on Entities with Audit Data *****/
    ConfigureAuditableEntity(builder.Entity<BeanBatch>());
    ConfigureAuditableEntity(builder.Entity<UserBrewSetup>());
    ConfigureAuditableEntity(builder.Entity<BrewSetting>());
    ConfigureAuditableEntity(builder.Entity<BrewerUserSetting>());
    ConfigureAuditableEntity(builder.Entity<BrewGrinderDialSetting>());
    ConfigureAuditableEntity(builder.Entity<BrewerUserSetting>());

    /***** Setup Relations on Entities *****/

    // User > UserRoles for User <-> Roles
    builder.Entity<User>()
      .HasMany(u => u.UserRoles)
      .WithOne(ur => ur.User)
      .HasForeignKey(ur => ur.UserId)
      .IsRequired();

    // Role > UserRoles for User <-> Roles
    builder.Entity<Role>()
      .HasMany(r => r.UserRoles)
      .WithOne(ur => ur.Role)
      .HasForeignKey(ur => ur.RoleId)
      .IsRequired();

    // User > User (possibly self ref for audit tracking)
    builder.Entity<User>()
      .HasOne(u => u.UpdatedBy)
      .WithMany()
      .HasForeignKey(u => u.UpdatedById)
      .OnDelete(DeleteBehavior.SetNull);

    // Roaster (composite unique on name, location)
    builder.Entity<Roaster>()
      .HasIndex(r => new { r.Name, r.Location })
      .IsUnique();

    // Bean (composite unique on roasterId, name)
    builder.Entity<Bean>()
      .HasIndex(b => new { b.RoasterId, b.Name })
      .IsUnique();

    // Bean > Roaster
    builder.Entity<Bean>()
      .HasOne(b => b.Roaster)
      .WithMany(r => r.Beans)
      .HasForeignKey(b => b.RoasterId)
      .OnDelete(DeleteBehavior.SetNull);

    // BeanBatch > Bean
    builder.Entity<BeanBatch>()
      .HasOne(bb => bb.Bean)
      .WithMany(b => b.BeanBatches)
      .HasForeignKey(bb => bb.BeanId)
      .OnDelete(DeleteBehavior.Cascade); // BeanBatches cannot exist without the corresponding Bean

    // BeanBatch > User
    builder.Entity<BeanBatch>()
      .HasOne(bb => bb.User)
      .WithMany(u => u.BeanBatches)
      .HasForeignKey(bb => bb.UserId)
      .OnDelete(DeleteBehavior.Restrict); // Will stop the deletion of user if they still have BeanBatches.
    // When a user is deleted, we should give them the option to preserve their BeanBatch data
    // If they allow preservation of BeanBatch data, we reassign the BeanBatch to an Archived User's ID. (create one of these users)
    // Then delete the User when everything is moved.
    // Do this similarly for anything that has User FK.
    // For BrewSettings, we can allow the Archived User to have multiple recommended.

    // BrewMethod (unique on name)
    builder.Entity<BrewMethod>()
      .HasIndex(bm => bm.Name)
      .IsUnique();

    // Brewers > BrewMethod
    builder.Entity<Brewer>()
      .HasOne(b => b.BrewMethod)
      .WithMany(bm => bm.Brewers)
      .HasForeignKey(b => b.BrewMethodId)
      .OnDelete(DeleteBehavior.SetNull);

    // Brewers > Brand
    builder.Entity<Brewer>()
      .HasOne(b => b.Brand)
      .WithMany(bd => bd.Brewers)
      .HasForeignKey(b => b.BrandId)
      .OnDelete(DeleteBehavior.SetNull);

    // Grinder > Brand
    builder.Entity<Grinder>()
      .HasOne(g => g.Brand)
      .WithMany(b => b.Grinders)
      .HasForeignKey(g => g.BrandId)
      .OnDelete(DeleteBehavior.SetNull);

    // GrinderElementCompatibility composite PK
    builder.Entity<GrinderElementCompatibility>()
      .HasKey(gec => new { gec.GrinderId, gec.GrindingElementId });

    // GrinderElementCompatibility > Grinder
    builder.Entity<GrinderElementCompatibility>()
      .HasOne(gec => gec.Grinder)
      .WithMany(g => g.CompatibleParts)
      .HasForeignKey(gec => gec.GrinderId)
      .OnDelete(DeleteBehavior.Cascade);

    // GrinderElementCompatibility > GrindingElement
    builder.Entity<GrinderElementCompatibility>()
      .HasOne(gec => gec.GrindingElement)
      .WithMany(ge => ge.CompatibleGrinders)
      .HasForeignKey(gec => gec.GrindingElementId)
      .OnDelete(DeleteBehavior.Cascade);

    // GrindingElement > GrindingMechanism
    builder.Entity<GrindingElement>()
      .HasOne(ge => ge.GrindingMechanism)
      .WithMany(gm => gm.GrinderParts)
      .HasForeignKey(ge => ge.GrindingMechanismId)
      .OnDelete(DeleteBehavior.Restrict);

    // BrewSetup composite key
    builder.Entity<BrewSetup>()
      .HasIndex(bs => new { bs.GrinderId, bs.BrewerId, bs.BeanId })
      .IsUnique();

    // BrewSetup > Grinder
    builder.Entity<BrewSetup>()
      .HasOne(bs => bs.Grinder)
      .WithMany(g => g.BrewSetups)
      .HasForeignKey(bs => bs.GrinderId)
      .OnDelete(DeleteBehavior.Restrict);

    // BrewSetup > Brewer
    builder.Entity<BrewSetup>()
      .HasOne(bs => bs.Brewer)
      .WithMany(b => b.BrewSetups)
      .HasForeignKey(bs => bs.BrewerId)
      .OnDelete(DeleteBehavior.Restrict);

    // BrewSetup > Bean
    builder.Entity<BrewSetup>()
      .HasOne(bs => bs.Bean)
      .WithMany(b => b.BrewSetups)
      .HasForeignKey(bs => bs.BeanId)
      .OnDelete(DeleteBehavior.Restrict);

    // UserBrewSetup composite key
    builder.Entity<UserBrewSetup>()
      .HasKey(ubs => new { ubs.UserId, ubs.BrewSetupId });

    // UserBrewSetup > User
    builder.Entity<UserBrewSetup>()
      .HasOne(ubs => ubs.User)
      .WithMany(u => u.UserBrewSetups)
      .HasForeignKey(ubs => ubs.UserId)
      .OnDelete(DeleteBehavior.Restrict);

    // UserBrewSetup > BrewSetup
    builder.Entity<UserBrewSetup>()
      .HasOne(ubs => ubs.BrewSetup)
      .WithMany(u => u.UserBrewSetups)
      .HasForeignKey(ubs => ubs.BrewSetupId)
      .OnDelete(DeleteBehavior.Restrict);

    // GrinderDial > Grinder
    builder.Entity<GrinderDial>()
      .HasOne(gd => gd.Grinder)
      .WithMany(g => g.GrinderDials)
      .HasForeignKey(gd => gd.GrinderId)
      .OnDelete(DeleteBehavior.Restrict);

    // BrewGrinderDialSetting composite key
    builder.Entity<BrewGrinderDialSetting>()
      .HasKey(bgds => new { bgds.BrewSettingId, bgds.GrinderDialId });

    // BrewGrinderDialSetting > GrinderDial
    builder.Entity<BrewGrinderDialSetting>()
      .HasOne(bgds => bgds.GrinderDial)
      .WithMany(gd => gd.BrewGrinderDialSettings)
      .HasForeignKey(bgds => bgds.GrinderDialId)
      .OnDelete(DeleteBehavior.Restrict);

    // BrewGrinderDialSetting > BrewSetting
    builder.Entity<BrewGrinderDialSetting>()
      .HasOne(bgds => bgds.BrewSetting)
      .WithMany(bs => bs.BrewGrinderDialSettings)
      .HasForeignKey(bgds => bgds.BrewSettingId)
      .OnDelete(DeleteBehavior.Cascade); // If the user deletes the BrewSetting, BrewGrinderDialSetting becomes useless

    // For each User, only have one recommended BrewSetting per BrewSetup
    builder.Entity<BrewSetting>()
      .HasIndex(bs => new { bs.UserId, bs.BrewSetupId })
      .HasFilter("Recommended = 1") // SQLite
                                    // .HasFilter("\"Recommended\" = TRUE") // PostgreSQL
      .IsUnique();

    // BrewSettings > User
    builder.Entity<BrewSetting>()
      .HasOne(bs => bs.User)
      .WithMany(u => u.BrewSettings)
      .HasForeignKey(bs => bs.UserId)
      .OnDelete(DeleteBehavior.Restrict);

    // BrewSettings > BrewSetup
    builder.Entity<BrewSetting>()
      .HasOne(bs => bs.BrewSetup)
      .WithMany(bsu => bsu.BrewSettings)
      .HasForeignKey(bs => bs.BrewSetupId)
      .OnDelete(DeleteBehavior.Restrict);

    // BrewSettings > BeanBatch
    builder.Entity<BrewSetting>()
      .HasOne(bs => bs.BeanBatch)
      .WithMany(bb => bb.BrewSettings)
      .HasForeignKey(bs => bs.BeanBatchId)
      .OnDelete(DeleteBehavior.Restrict);

    // BrewerStockSetting > Brewer (eg. Double Shot button, Single Shot button on Bambino Plus)
    builder.Entity<BrewerStockSetting>()
      .HasOne(bss => bss.Brewer)
      .WithMany(u => u.BrewerStockSettings)
      .HasForeignKey(bss => bss.BrewerId)
      .OnDelete(DeleteBehavior.Restrict);

    // BrewerUserSetting > User
    builder.Entity<BrewerUserSetting>()
      .HasOne(bus => bus.User)
      .WithMany(u => u.BrewerUserSettings)
      .HasForeignKey(bus => bus.UserId)
      .OnDelete(DeleteBehavior.Restrict);

    // BrewerUserSetting > Brewer
    builder.Entity<BrewerUserSetting>()
      .HasOne(bus => bus.Brewer)
      .WithMany(b => b.BrewerUserSettings)
      .HasForeignKey(bus => bus.BrewerId)
      .OnDelete(DeleteBehavior.Restrict);
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
