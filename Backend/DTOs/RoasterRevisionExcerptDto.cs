namespace Backend.DTOs;

public class RoasterRevisionExcerptDto
{
  public int Id { get; set; }
  public string? Comment { get; set; }
  public int? Version { get; set; }
  public int? ParentRevisionId { get; set; }
}
