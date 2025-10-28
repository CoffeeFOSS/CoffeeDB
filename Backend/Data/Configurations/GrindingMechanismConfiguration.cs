using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class GrindingMechanismConfiguration : IEntityTypeConfiguration<GrindingMechanism>
{
  public void Configure(EntityTypeBuilder<GrindingMechanism> builder)
  {
    builder.Property(gm => gm.Name).HasMaxLength(50);
  }
}
