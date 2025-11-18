using Backend.Data.Configurations.Property;
using Backend.Entities.Revision;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations.Revision;

public class RoasterRevisionConfiguration : IEntityTypeConfiguration<RoasterRevision>
{
  public void Configure(EntityTypeBuilder<RoasterRevision> builder)
  {
    RoasterPropertyConfiguration.ApplyCommonProperties(builder);

    builder.HasIndex(rr => rr.RevisionMetadataId).IsUnique();
  }
}
