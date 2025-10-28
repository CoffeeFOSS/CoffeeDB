using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class BeanBatchConfiguration : IEntityTypeConfiguration<BeanBatch>
{
  public void Configure(EntityTypeBuilder<BeanBatch> builder)
  {
    // BeanBatch > Bean
    builder
      .HasOne(bb => bb.Bean)
      .WithMany(b => b.BeanBatches)
      .HasForeignKey(bb => bb.BeanId)
      .OnDelete(DeleteBehavior.Cascade); // BeanBatches cannot exist without the corresponding Bean

    // BeanBatch > User
    builder
      .HasOne(bb => bb.User)
      .WithMany(u => u.BeanBatches)
      .HasForeignKey(bb => bb.UserId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
