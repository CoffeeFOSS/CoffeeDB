using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class BeanConfiguration : IEntityTypeConfiguration<Bean>
{
  public void Configure(EntityTypeBuilder<Bean> builder)
  {
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
