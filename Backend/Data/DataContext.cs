using Backend.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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
  public DbSet<Roaster> Roasters { get; set; }
  public DbSet<Bean> Beans { get; set; }
  public DbSet<BrewMethod> BrewMethods { get; set; }
  public DbSet<Brewer> Brewers { get; set; }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    /*
    > Many to One
    < One to Many
    <-> Many to Many
    */

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
      .OnDelete(DeleteBehavior.NoAction);
  }
}
