using Backend.Entities.Abstract;
using Backend.Enums;

namespace Backend.Entities.Revision;

public class EntityRevision : BaseAuditableEntity
{
  public RevisionStatus Status { get; set; }
  public int? ParentRevisionId { get; set; }
  public int? Version { get; set; }
  public string? Comment { get; set; }

  public EntityRevision ParentRevision { get; set; } = null!;
  public ICollection<RoasterRevision> RoasterRevisions { get; set; } = null!;
}
