namespace Backend.Entities;

public class BrewSetup : BaseEntity
{
  public int GrinderId { get; set; }
  public Grinder Grinder { get; set; } = null!;
  public int BrewerId { get; set; }
  public Brewer Brewer { get; set; } = null!;
  public int BeanId { get; set; }
  public Bean Bean { get; set; } = null!;
  public ICollection<UserBrewSetup> UserBrewSetups { get; set; } = [];
}
