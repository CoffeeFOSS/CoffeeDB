using Backend.Data.Configurations.Property;
using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class RoasterConfiguration : IEntityTypeConfiguration<Roaster>
{
  public void Configure(EntityTypeBuilder<Roaster> builder)
  {
    RoasterPropertyConfiguration.ApplyCommonProperties(builder);

    // Roaster (composite unique on name, locationAddress)
    builder
      .HasIndex(r => new { r.Name, r.LocationAddress })
      .IsUnique();

    builder.HasIndex(r => r.LocationCoordinates).HasMethod("GIST");
  }
}
