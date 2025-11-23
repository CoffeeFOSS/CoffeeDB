using Backend.Entities.Abstract;

namespace Backend.Entities;

public class BrewerUserSetting : BaseAuditableEntity
{
  public string Name { get; set; } = string.Empty;
  public float? WaterTemperature { get; set; }
  public float? WaterVolume { get; set; }
  public float? BrewTime { get; set; }

  public int UserId { get; set; }
  public int BrewerId { get; set; }

  public User User { get; set; } = null!;
  public Brewer Brewer { get; set; } = null!;
}
