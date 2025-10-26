namespace Backend.Entities;

public class Grinder : BaseEntity
{
  public string Model { get; set; } = string.Empty;
  public string? ModelAlias { get; set; }
  public string? Brand { get; set; }
  public string? BrandAlias { get; set; }
  public string? Description { get; set; }
  public int? ReleaseDate { get; set; }

  public ICollection<GrinderBurr> CompatibleBurrs { get; set; } = [];
  public ICollection<GrinderDial> GrinderDials { get; set; } = [];
  public ICollection<BrewSetup> BrewSetups { get; set; } = [];
}
