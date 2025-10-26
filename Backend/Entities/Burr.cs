namespace Backend.Entities;

public class Burr : BaseEntity
{
  public string Type { get; set; } = string.Empty;
  public decimal Diameter { get; set; }

  public ICollection<GrinderBurr> CompatibleGrinders { get; set; } = [];
}
