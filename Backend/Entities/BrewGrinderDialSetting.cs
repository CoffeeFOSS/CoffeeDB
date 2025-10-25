namespace Backend.Entities;

public class BrewGrinderDialSetting
{
  public int BrewSettingId { get; set; }
  public BrewSetting BrewSetting { get; set; } = null!;
  public int GrinderDialId { get; set; }
  public GrinderDial GrinderDial { get; set; } = null!;
  public decimal DialValue { get; set; }
}

