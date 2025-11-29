using Backend.Entities.Abstract;

namespace Backend.Entities;

public class BeanBatch : BaseAuditableEntity
{
  public DateOnly RoastDate { get; set; }

  public int UserId { get; set; }
  public int BeanId { get; set; }

  public User User { get; set; } = null!;
  public Bean Bean { get; set; } = null!;
  public ICollection<BrewSetting> BrewSettings { get; set; } = [];
}
