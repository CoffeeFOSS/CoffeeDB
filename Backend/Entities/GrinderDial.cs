namespace Backend.Entities;

public class GrinderDial : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public decimal? Min { get; set; }
  public decimal? Max { get; set; }
  public decimal? Step { get; set; }
  public string? Note { get; set; }

  public int GrinderId { get; set; }

  public Grinder Grinder { get; set; } = null!;
  public ICollection<BrewGrinderDialSetting> BrewGrinderDialSettings { get; set; } = [];
}
