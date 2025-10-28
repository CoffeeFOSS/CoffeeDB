namespace Backend.DTOs;

public class RoasterDto
{
  public required int Id { get; set; }
  public required string Name { get; set; }
  public required string Alias { get; set; }
  public required string Location { get; set; }
  public required string WebsiteUrl { get; set; }
  public required string Description { get; set; }
}
