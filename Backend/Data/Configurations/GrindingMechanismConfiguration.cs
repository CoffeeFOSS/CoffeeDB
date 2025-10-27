using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class GrindingMechanismConfiguration : IEntityTypeConfiguration<GrindingMechanism>
{
  public void Configure(EntityTypeBuilder<GrindingMechanism> builder)
  {
    // this function is intentionally left blank
  }
}
