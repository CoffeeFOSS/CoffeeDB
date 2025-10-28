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
          "([WaterTemperature] IS NULL) OR ([WaterTemperature] >= 0)"
        );
        tb.HasCheckConstraint(
          "CK_BrewerUserSetting_WaterVolume_Positive",
          "([WaterVolume] IS NULL) OR ([WaterVolume] >= 0)"
        );
        tb.HasCheckConstraint(
          "CK_BrewerUserSetting_BrewTime_Positive",
          "([BrewTime] IS NULL) OR ([BrewTime] >= 0)"
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
