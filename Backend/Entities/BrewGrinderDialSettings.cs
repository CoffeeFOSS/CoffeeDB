namespace Backend.Entities;

public class BrewGrinderDialSettings
{
  // public int BrewSettingsId { get; set; }
  // public BrewSetting BrewSetting { get; set; }
  public int GrinderDialId { get; set; }
  public GrinderDial GrinderDial { get; set; } = null!;
  public decimal DialValue { get; set; }
}

