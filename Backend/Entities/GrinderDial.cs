namespace Backend.Entities;

public class GrinderDial : BaseEntity
{

  public int GrinderId { get; set; }
  public Grinder Grinder { get; set; } = null!;
  public string Name { get; set; } = string.Empty;
  public decimal? Min { get; set; }
  public decimal? Max { get; set; }
  public decimal? Step { get; set; }
  public string? Comment { get; set; }
  public ICollection<BrewGrinderDialSettings> BrewGrinderDialSettings { get; set; } = [];
}
