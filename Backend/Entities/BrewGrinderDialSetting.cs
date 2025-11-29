using Backend.Entities.Abstract;

namespace Backend.Entities;

public class BrewGrinderDialSetting : AuditableEntity
{
  public float DialValue { get; set; }

  public int BrewSettingId { get; set; }
  public int GrinderDialId { get; set; }

  public BrewSetting BrewSetting { get; set; } = null!;
  public GrinderDial GrinderDial { get; set; } = null!;
}

