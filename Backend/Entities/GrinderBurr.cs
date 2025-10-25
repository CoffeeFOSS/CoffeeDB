namespace Backend.Entities;

public class GrinderBurr
{
  public int GrinderId { get; set; }
  public int BurrId { get; set; }

  public Grinder Grinder { get; set; } = null!;
  public Burr Burr { get; set; } = null!;
}
