using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class BrewersConfiguration : IEntityTypeConfiguration<Brewer>
{
  public void Configure(EntityTypeBuilder<Brewer> builder)
  {
    // Brewers > BrewMethod
    builder
      .HasOne(b => b.BrewMethod)
      .WithMany(bm => bm.Brewers)
      .HasForeignKey(b => b.BrewMethodId)
      .OnDelete(DeleteBehavior.SetNull);

    // Brewers > Brand
    builder
      .HasOne(b => b.Brand)
      .WithMany(bd => bd.Brewers)
      .HasForeignKey(b => b.BrandId)
      .OnDelete(DeleteBehavior.SetNull);
  }
}
