namespace Backend.Entities;

public class Grinder : BaseEntity
{
  public string Model { get; set; } = string.Empty;
  public string? ModelAlias { get; set; }
  public string? Description { get; set; }
  public int? ReleaseDate { get; set; }

  public int? BrandId { get; set; }

  public Brand? Brand { get; set; } = null!;
  public ICollection<GrinderElementCompatibility> CompatibleParts { get; set; } = [];
  public ICollection<GrinderDial> GrinderDials { get; set; } = [];
  public ICollection<BrewSetup> BrewSetups { get; set; } = [];
}
