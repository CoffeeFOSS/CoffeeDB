using Backend.Entities.Abstract;

namespace Backend.Entities;

public class BrewerStockSetting : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public float? WaterTemperature { get; set; }
  public float? WaterVolume { get; set; }
  public float? BrewTime { get; set; }

  public int BrewerId { get; set; }

  public Brewer Brewer { get; set; } = null!;
}
