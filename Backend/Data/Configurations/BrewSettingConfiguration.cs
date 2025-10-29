using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class BrewSettingConfiguration : IEntityTypeConfiguration<BrewSetting>
{
  public void Configure(EntityTypeBuilder<BrewSetting> builder)
  {
    var doseCol = builder.GetColumnName(bs => bs.Dose);
    var grindTimeCol = builder.GetColumnName(bs => bs.GrindTime);
    var recommendedCol = builder.GetColumnName(bs => bs.Recommended);

    // At least one of Dose or GrindTime must be provided
    builder
      .ToTable(tb => tb.HasCheckConstraint(
        "CHK_BrewSetting_DoseOrGrindTime",
        $"({doseCol} IS NOT NULL) OR ({grindTimeCol} IS NOT NULL)"
      ));

    // For each User, only have one recommended BrewSetting per BrewSetup
    builder
      .HasIndex(bs => new { bs.UserId, bs.BrewSetupId })
      // .HasFilter("Recommended = 1") // SQLite
      .HasFilter($"{recommendedCol} = TRUE") // PostgreSQL
      .IsUnique();

    // BrewSettings > User
    builder
      .HasOne(bs => bs.User)
      .WithMany(u => u.BrewSettings)
      .HasForeignKey(bs => bs.UserId)
      .OnDelete(DeleteBehavior.Restrict);

    // BrewSettings > BrewSetup
    builder
      .HasOne(bs => bs.BrewSetup)
      .WithMany(bsu => bsu.BrewSettings)
      .HasForeignKey(bs => bs.BrewSetupId)
      .OnDelete(DeleteBehavior.Restrict);

    // BrewSettings > BeanBatch
    builder
      .HasOne(bs => bs.BeanBatch)
      .WithMany(bb => bb.BrewSettings)
      .HasForeignKey(bs => bs.BeanBatchId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
