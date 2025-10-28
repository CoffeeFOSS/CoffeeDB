using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class BrewerStockSettingConfiguration : IEntityTypeConfiguration<BrewerStockSetting>
{
  public void Configure(EntityTypeBuilder<BrewerStockSetting> builder)
  {
    builder.Property(b => b.Name).HasMaxLength(100);

    builder.ToTable(tb =>
      {
        tb.HasCheckConstraint(
          "CK_BrewerStockSetting_WaterTemperature_Positive",
          "[WaterTemperature] >= 0"
        );
        tb.HasCheckConstraint(
          "CK_BrewerStockSetting_WaterVolume_Positive",
          "[WaterVolume] >= 0"
        );
        tb.HasCheckConstraint(
          "CK_BrewerStockSetting_BrewTime_Positive",
          "[BrewTime] >= 0"
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
