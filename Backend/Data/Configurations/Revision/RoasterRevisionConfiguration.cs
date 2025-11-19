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

    // RoasterRevision > Roaster
    builder
      .HasOne(rr => rr.Roaster)
      .WithMany(r => r.Revisions)
      .HasForeignKey(rr => rr.RoasterId)
      .OnDelete(DeleteBehavior.SetNull);

    // RoasterRevision - RevisionMetadata
    builder
      .HasOne(rr => rr.RevisionMetadata)
      .WithOne(rm => rm.RoasterRevision)
      .HasForeignKey<RoasterRevision>(rr => rr.RevisionMetadataId)
      .OnDelete(DeleteBehavior.Cascade); // Delete RoasterRevision if RevisionMetadata is deleted
    // EFcore sees RevisionMetadata as the principal, and RoasterMetadata as the dependent.
    // So if I want to delete RoasterRevision, I should delete the RevisionMetadata instead.
    // Doing so will delete the RevisionMetadata and then it will cascade and delete the RoasterRevision as well.
    // Deleting RoasterRevision is going to orphan the RevisionMetadata.

    builder.HasIndex(rr => rr.RevisionMetadataId);
  }
}
