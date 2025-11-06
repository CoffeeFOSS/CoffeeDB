namespace Backend.Entities;

public class GrinderDial : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public float? Min { get; set; }
  public float? Max { get; set; }
  public float? Step { get; set; }
  public string? Note { get; set; }

  public int GrinderId { get; set; }

  public Grinder Grinder { get; set; } = null!;
  public ICollection<BrewGrinderDialSetting> BrewGrinderDialSettings { get; set; } = [];
}
