using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class GrinderElementCompatibilityConfiguration : IEntityTypeConfiguration<GrinderElementCompatibility>
{
  public void Configure(EntityTypeBuilder<GrinderElementCompatibility> builder)
  {
    // GrinderElementCompatibility composite PK
    builder
      .HasKey(gec => new { gec.GrinderId, gec.GrindingElementId });

    // GrinderElementCompatibility > Grinder
    builder
      .HasOne(gec => gec.Grinder)
      .WithMany(g => g.CompatibleParts)
      .HasForeignKey(gec => gec.GrinderId)
      .OnDelete(DeleteBehavior.Cascade);

    // GrinderElementCompatibility > GrindingElement
    builder
      .HasOne(gec => gec.GrindingElement)
      .WithMany(ge => ge.CompatibleGrinders)
      .HasForeignKey(gec => gec.GrindingElementId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
