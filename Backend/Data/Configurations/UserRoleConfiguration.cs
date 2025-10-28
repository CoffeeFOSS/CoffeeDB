using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
  public void Configure(EntityTypeBuilder<UserRole> builder)
  {
    // UserRole > User
    builder
      .HasOne(ur => ur.User)
      .WithMany(u => u.UserRoles)
      .HasForeignKey(ur => ur.UserId)
      .IsRequired();

    // UserRole > Role
    builder
      .HasOne(ur => ur.Role)
      .WithMany(r => r.UserRoles)
      .HasForeignKey(ur => ur.RoleId)
      .IsRequired();
  }
}
