using Backend.Entities.Abstract;

namespace Backend.Entities;

public class UserBrewSetup : BaseAuditableEntity
{
  public string? Name { get; set; }
  public string? Note { get; set; }

  public int UserId { get; set; }
  public int BrewSetupId { get; set; }

  public User User { get; set; } = null!;
  public BrewSetup BrewSetup { get; set; } = null!;
}
