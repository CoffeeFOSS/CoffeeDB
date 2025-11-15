using Backend.Entities.Abstract;

namespace Backend.Entities;

public class BrewSetup : BaseEntity
{
  public int GrinderId { get; set; }
  public int BrewerId { get; set; }
  public int BeanId { get; set; }

  public Grinder Grinder { get; set; } = null!;
  public Brewer Brewer { get; set; } = null!;
  public Bean Bean { get; set; } = null!;
  public ICollection<UserBrewSetup> UserBrewSetups { get; set; } = [];
  public ICollection<BrewSetting> BrewSettings { get; set; } = [];
}
