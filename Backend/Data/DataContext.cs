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
  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    // User <-> UserRoles
    builder.Entity<User>()
      .HasMany(u => u.UserRoles)
      .WithOne(ur => ur.User)
      .HasForeignKey(ur => ur.UserId)
      .IsRequired();

    builder.Entity<Role>()
      .HasMany(r => r.UserRoles)
      .WithOne(ur => ur.Role)
      .HasForeignKey(ur => ur.RoleId)
      .IsRequired();

    // User -> User (possibly self ref for audit tracking)
    builder.Entity<User>()
      .HasOne(u => u.UpdatedBy)
      .WithMany()
      .HasForeignKey(u => u.UpdatedById)
      .OnDelete(DeleteBehavior.SetNull); // if admin edits rob, rob will have UpdatedById = 1, if admin gets deleted, the UpdatedById on rob gets dereferenced
  }
}
