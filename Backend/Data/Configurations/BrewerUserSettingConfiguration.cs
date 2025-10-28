using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class BrewerUserSettingConfiguration : IEntityTypeConfiguration<BrewerUserSetting>
{
  public void Configure(EntityTypeBuilder<BrewerUserSetting> builder)
  {
    builder.Property(bus => bus.Name).HasMaxLength(100);

    builder.ToTable(tb =>
      {
        tb.HasCheckConstraint(
          "CK_BrewerUserSetting_WaterTemperature_Positive",
          $"({nameof(BrewerUserSetting.WaterTemperature)} IS NULL) OR ({nameof(BrewerUserSetting.WaterTemperature)} >= 0)"
        );
        tb.HasCheckConstraint(
          "CK_BrewerUserSetting_WaterVolume_Positive",
          $"({nameof(BrewerUserSetting.WaterVolume)} IS NULL) OR ({nameof(BrewerUserSetting.WaterVolume)} >= 0)"
        );
        tb.HasCheckConstraint(
          "CK_BrewerUserSetting_BrewTime_Positive",
          $"({nameof(BrewerUserSetting.BrewTime)} IS NULL) OR ({nameof(BrewerUserSetting.BrewTime)} >= 0)"
        );
      }
    );

    // BrewerUserSetting > User
    builder
      .HasOne(bus => bus.User)
      .WithMany(u => u.BrewerUserSettings)
      .HasForeignKey(bus => bus.UserId)
      .OnDelete(DeleteBehavior.Restrict);

    // BrewerUserSetting > Brewer
    builder
      .HasOne(bus => bus.Brewer)
      .WithMany(b => b.BrewerUserSettings)
      .HasForeignKey(bus => bus.BrewerId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
