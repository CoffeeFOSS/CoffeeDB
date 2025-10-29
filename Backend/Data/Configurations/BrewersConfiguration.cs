using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class BrewersConfiguration : IEntityTypeConfiguration<Brewer>
{
  public void Configure(EntityTypeBuilder<Brewer> builder)
  {
    builder.Property(b => b.Model).HasMaxLength(100);
    builder.Property(b => b.ModelAlias).HasMaxLength(100);
    builder.Property(b => b.Description).HasMaxLength(2000);

    var releaseDateCol = builder.GetColumnName(b => b.ReleaseDate);
    var waterCapacityCol = builder.GetColumnName(b => b.WaterCapacity);

    builder.ToTable(tb =>
      {
        tb.HasCheckConstraint(
            "CK_Brewer_ReleaseDate_8Digits",
            $"({releaseDateCol} IS NULL) OR ({releaseDateCol} >= 10000000 AND {releaseDateCol} <= 99999999)"
        );
        tb.HasCheckConstraint(
            "CK_Brewer_WaterCapacity_Positive",
            $"({waterCapacityCol} IS NULL) OR ({waterCapacityCol} > 0)"
        );
      }
    );

    // Brewers > BrewMethod
    builder
      .HasOne(b => b.BrewMethod)
      .WithMany(bm => bm.Brewers)
      .HasForeignKey(b => b.BrewMethodId)
      .OnDelete(DeleteBehavior.SetNull);

    // Brewers > Brand
    builder
      .HasOne(b => b.Brand)
      .WithMany(bd => bd.Brewers)
      .HasForeignKey(b => b.BrandId)
      .OnDelete(DeleteBehavior.SetNull);
  }
}
