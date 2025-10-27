using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class RoasterConfiguration : IEntityTypeConfiguration<Roaster>
{
  public void Configure(EntityTypeBuilder<Roaster> builder)
  {
    // Roaster (composite unique on name, location)
    builder
      .HasIndex(r => new { r.Name, r.Location })
      .IsUnique();
  }
}
