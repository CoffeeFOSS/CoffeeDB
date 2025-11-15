namespace Backend.DTOs;

public class RoasterRevisionExcerptDto
{
  public int Id { get; set; }
  public string Comment { get; set; } = null!;
  public string Status { get; set; } = null!;
  public int? Version { get; set; }
  public int? ParentRevisionId { get; set; }
}
