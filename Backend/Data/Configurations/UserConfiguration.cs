using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    // User > User (possibly self ref for audit tracking)
    builder
      .HasOne(u => u.UpdatedBy)
      .WithMany()
      .HasForeignKey(u => u.UpdatedById)
      .OnDelete(DeleteBehavior.SetNull);
  }
}
