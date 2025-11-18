using Backend.Entities.Revision;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations.Revision;

public class RevisionMetadataConfiguration : IEntityTypeConfiguration<RevisionMetadata>
{
  public void Configure(EntityTypeBuilder<RevisionMetadata> builder)
  {
    // anything to put here?
  }
}
