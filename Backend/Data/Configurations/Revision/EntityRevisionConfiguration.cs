using Backend.Entities.Revision;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations.Revision;

public class EntityRevisionConfiguration : IEntityTypeConfiguration<EntityRevision>
{
  public void Configure(EntityTypeBuilder<EntityRevision> builder)
  {
    // anything to put here?
  }
}
