using Backend.Entities.Abstract;

namespace Backend.Entities;

public class GrindingMechanism : BaseEntity
{
  public string Name { get; set; } = string.Empty;

  public ICollection<GrindingElement> GrinderParts { get; set; } = [];
}
