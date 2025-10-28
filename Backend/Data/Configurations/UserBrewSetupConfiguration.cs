using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class UserBrewSetupConfiguration : IEntityTypeConfiguration<UserBrewSetup>
{
  public void Configure(EntityTypeBuilder<UserBrewSetup> builder)
  {
    builder.Property(b => b.Name).HasMaxLength(100);
    builder.Property(b => b.Note).HasMaxLength(500);

    // UserBrewSetup composite key
    builder
      .HasKey(ubs => new { ubs.UserId, ubs.BrewSetupId });

    // UserBrewSetup > User
    builder
      .HasOne(ubs => ubs.User)
      .WithMany(u => u.UserBrewSetups)
      .HasForeignKey(ubs => ubs.UserId)
      .OnDelete(DeleteBehavior.Restrict);

    // UserBrewSetup > BrewSetup
    builder
      .HasOne(ubs => ubs.BrewSetup)
      .WithMany(u => u.UserBrewSetups)
      .HasForeignKey(ubs => ubs.BrewSetupId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
