using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class BrewGrinderDialSettingConfiguration : IEntityTypeConfiguration<BrewGrinderDialSetting>
{
  public void Configure(EntityTypeBuilder<BrewGrinderDialSetting> builder)
  {
    // BrewGrinderDialSetting composite key
    builder
      .HasKey(bgds => new { bgds.BrewSettingId, bgds.GrinderDialId });

    // BrewGrinderDialSetting > GrinderDial
    builder
      .HasOne(bgds => bgds.GrinderDial)
      .WithMany(gd => gd.BrewGrinderDialSettings)
      .HasForeignKey(bgds => bgds.GrinderDialId)
      .OnDelete(DeleteBehavior.Restrict);

    // BrewGrinderDialSetting > BrewSetting
    builder
      .HasOne(bgds => bgds.BrewSetting)
      .WithMany(bs => bs.BrewGrinderDialSettings)
      .HasForeignKey(bgds => bgds.BrewSettingId)
      .OnDelete(DeleteBehavior.Cascade); // If the user deletes the BrewSetting, BrewGrinderDialSetting becomes useless
  }
}
