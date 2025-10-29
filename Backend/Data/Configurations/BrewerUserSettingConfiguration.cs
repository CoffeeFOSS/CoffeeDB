using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class BrewerUserSettingConfiguration : IEntityTypeConfiguration<BrewerUserSetting>
{
  public void Configure(EntityTypeBuilder<BrewerUserSetting> builder)
  {
    builder.Property(bus => bus.Name).HasMaxLength(100);

    var waterTemperatureCol = builder.GetColumnName(bus => bus.WaterTemperature);
    var waterVolumeCol = builder.GetColumnName(bus => bus.WaterVolume);
    var brewTimeCol = builder.GetColumnName(bus => bus.BrewTime);

    builder.ToTable(tb =>
      {
        tb.HasCheckConstraint(
          "CK_BrewerUserSetting_WaterTemperature_Positive",
          $"({waterTemperatureCol} IS NULL) OR ({waterTemperatureCol} >= 0)"
        );
        tb.HasCheckConstraint(
          "CK_BrewerUserSetting_WaterVolume_Positive",
          $"({waterVolumeCol} IS NULL) OR ({waterVolumeCol} >= 0)"
        );
        tb.HasCheckConstraint(
          "CK_BrewerUserSetting_BrewTime_Positive",
          $"({brewTimeCol} IS NULL) OR ({brewTimeCol} >= 0)"
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
