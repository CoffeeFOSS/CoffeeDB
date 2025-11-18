namespace Backend.DTOs;

public class RoasterRevisionSnapshotDto : RoasterDto
{
  public string Comment { get; set; } = null!;
  public string Status { get; set; } = null!;
  public int? RoasterId { get; set; }
  public int? Version { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }

  public string? CreatedBy { get; set; }
  public string? UpdatedBy { get; set; }
}
