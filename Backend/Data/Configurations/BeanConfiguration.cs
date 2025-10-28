using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class BeanConfiguration : IEntityTypeConfiguration<Bean>
{
  public void Configure(EntityTypeBuilder<Bean> builder)
  {
    builder.Property(b => b.Name).HasMaxLength(100);
    builder.Property(b => b.Alias).HasMaxLength(200);
    builder.Property(b => b.Region).HasMaxLength(100);
    builder.Property(b => b.Farm).HasMaxLength(100);
    builder.Property(b => b.WetMill).HasMaxLength(100);
    builder.Property(b => b.Varietal).HasMaxLength(100);
    builder.Property(b => b.Producer).HasMaxLength(100);
    builder.Property(b => b.Importer).HasMaxLength(100);
    builder.Property(b => b.Process).HasMaxLength(100);
    builder.Property(b => b.FlavorProfile).HasMaxLength(100);

    builder.ToTable(tb => tb.HasCheckConstraint(
        "CK_Bean_ReleaseDate_8Digits",
        "([ReleaseDate] IS NULL) OR ([ReleaseDate] >= 10000000 AND [ReleaseDate] <= 99999999)"
    ));

    // Bean (composite unique on roasterId, name)
    builder
      .HasIndex(b => new { b.RoasterId, b.Name })
      .IsUnique();

    // Bean > Roaster
    builder
      .HasOne(b => b.Roaster)
      .WithMany(r => r.Beans)
      .HasForeignKey(b => b.RoasterId)
      .OnDelete(DeleteBehavior.SetNull);
  }
}
