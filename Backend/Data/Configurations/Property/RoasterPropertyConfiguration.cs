using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations.Property;

public static class RoasterPropertyConfiguration
{
  public static void ApplyCommonProperties<T>(EntityTypeBuilder<T> builder) where T : class
  {
    builder.Property("Name").HasMaxLength(100);
    builder.Property("Alias").HasMaxLength(200);
    builder.Property("LocationAddress").HasMaxLength(500);
    builder.Property("Description").HasMaxLength(2000);

    // FQDN + some leeway for directories https://en.wikipedia.org/wiki/Fully_qualified_domain_name
    builder.Property("WebsiteUrl").HasMaxLength(300);

    // PostGIS Coordinate Data
    builder.Property("LocationCoordinates").HasColumnType("geography (point, 4326)");
  }
}
