using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class GrinderConfiguration : IEntityTypeConfiguration<Grinder>
{
  public void Configure(EntityTypeBuilder<Grinder> builder)
  {
    builder.Property(g => g.Model).HasMaxLength(100);
    builder.Property(g => g.ModelAlias).HasMaxLength(200);
    builder.Property(g => g.Description).HasMaxLength(2000);

    builder.ToTable(tb => tb.HasCheckConstraint(
      "CK_Grinder_ReleaseDate_8Digits",
      $"({nameof(Grinder.ReleaseDate)} IS NULL) OR ({nameof(Grinder.ReleaseDate)} >= 10000000 AND {nameof(Grinder.ReleaseDate)} <= 99999999)"
    ));

    // Grinder > Brand
    builder
      .HasOne(g => g.Brand)
      .WithMany(b => b.Grinders)
      .HasForeignKey(g => g.BrandId)
      .OnDelete(DeleteBehavior.SetNull);
  }
}
