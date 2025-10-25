namespace Backend.Entities;

public class BrewerStockSetting : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public decimal? WaterTemperature { get; set; }
  public decimal? WaterVolume { get; set; }
  public decimal? BrewTime { get; set; }

  public int BrewerId { get; set; }

  public Brewer Brewer { get; set; } = null!;
}
