namespace Backend.Entities;

public class Brewer : BaseEntity
{
  public string Model { get; set; } = string.Empty;
  public string? ModelAlias { get; set; }
  public string? Brand { get; set; }
  public string? BrandAlias { get; set; }
  public int BrewMethodId { get; set; }
  public BrewMethod BrewMethod { get; set; } = null!;
  public DateOnly? ReleaseDate { get; set; }
  public string? Description { get; set; }
  public ICollection<BrewSetup> BrewSetups { get; set; } = [];
}
