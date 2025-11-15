using Backend.Entities.Abstract;

namespace Backend.DTOs;

public class EntityRevisionDto : BaseAuditableEntity
{
  public string Status { get; set; } = null!;
  public int? ParentRevisionId { get; set; }
  public int? Version { get; set; }
  public string? Comment { get; set; }

  public string EntityType { get; set; } = default!;
}
