using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class BrewSetupConfiguration : IEntityTypeConfiguration<BrewSetup>
{
  public void Configure(EntityTypeBuilder<BrewSetup> builder)
  {
    // BrewSetup composite key
    builder
      .HasIndex(bs => new { bs.GrinderId, bs.BrewerId, bs.BeanId })
      .IsUnique();

    // BrewSetup > Grinder
    builder
      .HasOne(bs => bs.Grinder)
      .WithMany(g => g.BrewSetups)
      .HasForeignKey(bs => bs.GrinderId)
      .OnDelete(DeleteBehavior.Restrict);

    // BrewSetup > Brewer
    builder
      .HasOne(bs => bs.Brewer)
      .WithMany(b => b.BrewSetups)
      .HasForeignKey(bs => bs.BrewerId)
      .OnDelete(DeleteBehavior.Restrict);

    // BrewSetup > Bean
    builder
      .HasOne(bs => bs.Bean)
      .WithMany(b => b.BrewSetups)
      .HasForeignKey(bs => bs.BeanId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
