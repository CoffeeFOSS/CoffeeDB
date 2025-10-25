namespace Backend.Entities;

public class Roaster : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public string? Aliases { get; set; }
  public string? Location { get; set; }
  public string? WebsiteUrl { get; set; }
  public string? Description { get; set; }
}
