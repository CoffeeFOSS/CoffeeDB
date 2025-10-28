namespace Backend.Entities;

public class BrewerUserSetting : BaseAuditableEntity
{
  public string Name { get; set; } = string.Empty;
  public decimal? WaterTemperature { get; set; }
  public decimal? WaterVolume { get; set; }
  public decimal? BrewTime { get; set; }

  public int UserId { get; set; }
  public int BrewerId { get; set; }

  public User User { get; set; } = null!;
  public Brewer Brewer { get; set; } = null!;
}
