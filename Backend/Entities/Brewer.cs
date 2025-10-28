namespace Backend.Entities;

public class Brewer : BaseEntity
{
  public string Model { get; set; } = string.Empty;
  public string? ModelAlias { get; set; }
  public int? ReleaseDate { get; set; }
  public string? Description { get; set; }
  public decimal? WaterCapacity { get; set; }

  public int BrewMethodId { get; set; }
  public int? BrandId { get; set; }

  public BrewMethod BrewMethod { get; set; } = null!;
  public Brand? Brand { get; set; } = null!;
  public ICollection<BrewSetup> BrewSetups { get; set; } = [];
  public ICollection<BrewerUserSetting> BrewerUserSettings { get; set; } = [];
  public ICollection<BrewerStockSetting> BrewerStockSettings { get; set; } = [];
}
