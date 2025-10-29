using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class BrewerStockSettingConfiguration : IEntityTypeConfiguration<BrewerStockSetting>
{
  public void Configure(EntityTypeBuilder<BrewerStockSetting> builder)
  {
    builder.Property(bss => bss.Name).HasMaxLength(100);

    var waterTemperatureCol = builder.GetColumnName(bss => bss.WaterTemperature);
    var waterVolumeCol = builder.GetColumnName(bss => bss.WaterVolume);
    var brewTimeCol = builder.GetColumnName(bss => bss.BrewTime);

    builder.ToTable(tb =>
      {
        tb.HasCheckConstraint(
          "CK_BrewerStockSetting_WaterTemperature_Positive",
          $"({waterTemperatureCol} IS NULL) OR ({waterTemperatureCol} >= 0)"
        );
        tb.HasCheckConstraint(
          "CK_BrewerStockSetting_WaterVolume_Positive",
          $"({waterVolumeCol} IS NULL) OR ({waterVolumeCol} >= 0)"
        );
        tb.HasCheckConstraint(
          "CK_BrewerStockSetting_BrewTime_Positive",
          $"({brewTimeCol} IS NULL) OR ({brewTimeCol} >= 0)"
        );
      }
    );

    // BrewerStockSetting > Brewer (eg. Double Shot button, Single Shot button on Bambino Plus)
    builder
      .HasOne(bss => bss.Brewer)
      .WithMany(u => u.BrewerStockSettings)
      .HasForeignKey(bss => bss.BrewerId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
