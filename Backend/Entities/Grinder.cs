namespace Backend.Entities;

public class Grinder : BaseEntity
{
  public string Model { get; set; } = string.Empty;
  public string? Description { get; set; }
  public string? ModelAlias { get; set; }
  public string? Brand { get; set; }
  public string? BrandAlias { get; set; }
  public DateOnly? ReleaseDate { get; set; }
  public ICollection<GrinderBurr> CompatibleBurrs { get; set; } = [];
}
