using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class GrinderConfiguration : IEntityTypeConfiguration<Grinder>
{
  public void Configure(EntityTypeBuilder<Grinder> builder)
  {
    // Grinder > Brand
    builder
      .HasOne(g => g.Brand)
      .WithMany(b => b.Grinders)
      .HasForeignKey(g => g.BrandId)
      .OnDelete(DeleteBehavior.SetNull);
  }
}
