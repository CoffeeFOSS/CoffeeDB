using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class GrindingElementConfiguration : IEntityTypeConfiguration<GrindingElement>
{
  public void Configure(EntityTypeBuilder<GrindingElement> builder)
  {
    // GrindingElement > GrindingMechanism
    builder
      .HasOne(ge => ge.GrindingMechanism)
      .WithMany(gm => gm.GrinderParts)
      .HasForeignKey(ge => ge.GrindingMechanismId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
