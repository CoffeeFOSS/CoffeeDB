using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class RoasterConfiguration : IEntityTypeConfiguration<Roaster>
{
  public void Configure(EntityTypeBuilder<Roaster> builder)
  {
    builder.Property(r => r.Name).HasMaxLength(100);
    builder.Property(r => r.Alias).HasMaxLength(200);
    builder.Property(r => r.Location).HasMaxLength(500);
    builder.Property(r => r.Description).HasMaxLength(2000);

    // FQDN + some leeway for directories https://en.wikipedia.org/wiki/Fully_qualified_domain_name
    builder.Property(r => r.WebsiteUrl).HasMaxLength(300);

    // Roaster (composite unique on name, location)
    builder
      .HasIndex(r => new { r.Name, r.Location })
      .IsUnique();
  }
}
