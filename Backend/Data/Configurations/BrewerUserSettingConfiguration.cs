using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class BrewerUserSettingConfiguration : IEntityTypeConfiguration<BrewerUserSetting>
{
  public void Configure(EntityTypeBuilder<BrewerUserSetting> builder)
  {
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
