using Backend.Entities.Abstract;
using Backend.Enums;

namespace Backend.Entities.Revision;

public class EntityRevision : BaseAuditableEntity
{
  // EntityRevision.Id are the same as RoasterRevision.Id, etc.
  // Meaning we could have this
  // RoasterRevision 1 EntityRevision 1
  // BrewerRevision  2 EntityRevision 2
  // BrewerRevision  3 EntityRevision 3
  // GrinderRevision 4 EntityRevision 4
  public RevisionStatus Status { get; set; }
  public int? ParentRevisionId { get; set; }
  public int? Version { get; set; }
  public string Comment { get; set; } = string.Empty;

  public EntityRevision ParentRevision { get; set; } = null!;
  public ICollection<RoasterRevision> RoasterRevisions { get; set; } = null!;
}
