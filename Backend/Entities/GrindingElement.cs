namespace Backend.Entities;

public class GrindingElement : BaseEntity
{
  public decimal Diameter { get; set; }

  public int GrindingMechanismId { get; set; }

  public GrindingMechanism GrindingMechanism { get; set; } = null!;
  public ICollection<GrinderElementCompatibility> CompatibleGrinders { get; set; } = [];
}
