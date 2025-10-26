namespace Backend.Entities;

public class GrinderElementCompatibility
{
  public int GrinderId { get; set; }
  public int GrindingElementId { get; set; }

  public Grinder Grinder { get; set; } = null!;
  public GrindingElement GrindingElement { get; set; } = null!;
}
