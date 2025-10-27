using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class GrinderDialConfiguration : IEntityTypeConfiguration<GrinderDial>
{
  public void Configure(EntityTypeBuilder<GrinderDial> builder)
  {
    // GrinderDial > Grinder
    builder
      .HasOne(gd => gd.Grinder)
      .WithMany(g => g.GrinderDials)
      .HasForeignKey(gd => gd.GrinderId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
