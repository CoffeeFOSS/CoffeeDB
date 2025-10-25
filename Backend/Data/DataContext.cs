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
  public DbSet<Roaster> Roasters { get; set; } = null!;
  public DbSet<Bean> Beans { get; set; } = null!;

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

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
  }
}
