namespace Backend.DTOs;

public class RoasterDto
{
  public required int Id { get; set; }
  public required string Name { get; set; }
  public string? Alias { get; set; }
  public string? Location { get; set; }
  public string? WebsiteUrl { get; set; }
  public string? Description { get; set; }
}
