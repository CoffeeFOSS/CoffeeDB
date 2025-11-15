using Backend.Entities;

namespace Backend.DTOs;

public class RoasterRevisionSnapshotDto : RoasterDto
{
  public int RoasterId { get; set; }
  public int EntityRevisionId { get; set; }
  public string? Comment { get; set; }
  public int? Version { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  public string? CreatedBy { get; set; } = null!;
  public string? UpdatedBy { get; set; } = null!;
}
