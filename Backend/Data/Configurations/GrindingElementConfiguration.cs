using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class GrindingElementConfiguration : IEntityTypeConfiguration<GrindingElement>
{
  public void Configure(EntityTypeBuilder<GrindingElement> builder)
  {
    builder.ToTable(tb => tb.HasCheckConstraint(
      "CK_GrindingElement_Diameter_Positive",
      $"{nameof(GrindingElement.Diameter)} >= 0"
    ));

    // GrindingElement > GrindingMechanism
    builder
      .HasOne(ge => ge.GrindingMechanism)
      .WithMany(gm => gm.GrinderParts)
      .HasForeignKey(ge => ge.GrindingMechanismId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
