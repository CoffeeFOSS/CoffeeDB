using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class BrewMethodConfiguration : IEntityTypeConfiguration<BrewMethod>
{
  public void Configure(EntityTypeBuilder<BrewMethod> builder)
  {
    // BrewMethod (unique on name)
    builder
      .HasIndex(bm => bm.Name)
      .IsUnique();
  }
}
