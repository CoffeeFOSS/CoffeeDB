namespace Backend.DTOs;

public class RoasterRevisionSnapshotDto : RoasterDto
{
  public int? RoasterId { get; set; }
  public string Comment { get; set; } = null!;
  public int? Version { get; set; }
  public string Status { get; set; } = null!;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  public string? CreatedBy { get; set; }
  public string? UpdatedBy { get; set; }
}
