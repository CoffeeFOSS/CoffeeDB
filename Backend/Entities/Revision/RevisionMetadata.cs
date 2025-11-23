using Backend.Entities.Abstract;
using Backend.Enums;

namespace Backend.Entities.Revision;

public class RevisionMetadata : BaseAuditableEntity
{
  // RevisionMetadata.Id are the same as RoasterRevision.Id, etc.
  // Meaning we could have this
  // RoasterRevision 1 RevisionMetadata 1
  // BrewerRevision  2 RevisionMetadata 2
  // BrewerRevision  3 RevisionMetadata 3
  // GrinderRevision 4 RevisionMetadata 4
  public RevisionStatus Status { get; set; }
  public int? ParentRevisionId { get; set; }
  public int? Version { get; set; }
  public string Comment { get; set; } = string.Empty;

  public RevisionMetadata ParentRevision { get; set; } = null!;

  // Navigation to exactly one of the revision types
  public RoasterRevision? RoasterRevision { get; set; } = null!;
}
