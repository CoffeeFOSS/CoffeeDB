namespace Backend.Entities;

public class BrewGrinderDialSetting : AuditableEntity
{
  public decimal DialValue { get; set; }

  public int BrewSettingId { get; set; }
  public int GrinderDialId { get; set; }

  public BrewSetting BrewSetting { get; set; } = null!;
  public GrinderDial GrinderDial { get; set; } = null!;
}

