using Backend.Entities.Revision;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations.Revision;

public class RevisionMetadataConfiguration : IEntityTypeConfiguration<RevisionMetadata>
{
  public void Configure(EntityTypeBuilder<RevisionMetadata> builder)
  {
    // anything to put here?
    // I'd like to make sure that for an Entity like Roaster, RoasterID cannot have 2 of the same versions, but thats hard to do here
  }
}
