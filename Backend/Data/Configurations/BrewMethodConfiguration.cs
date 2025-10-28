using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class BrewMethodConfiguration : IEntityTypeConfiguration<BrewMethod>
{
  public void Configure(EntityTypeBuilder<BrewMethod> builder)
  {
    builder.Property(bm => bm.Name).HasMaxLength(50);

    // BrewMethod (unique on name)
    builder
      .HasIndex(bm => bm.Name)
      .IsUnique();
  }
}
