using Backend.Entities.Abstract;

namespace Backend.DTOs;

public class EntityRevisionDto
{
  public int Id { get; set; }
  public string Status { get; set; } = null!;
  public int? ParentRevisionId { get; set; }
  public int? Version { get; set; }
  public string Comment { get; set; } = null!;

  public string EntityType { get; set; } = default!;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  public string? CreatedBy { get; set; } = null!;
  public string? UpdatedBy { get; set; } = null!;
}
