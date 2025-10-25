namespace Backend.Entities;

public class UserBrewSetup : BaseAuditableEntity
{
  public int UserId { get; set; }
  public User User { get; set; } = null!;
  public int BrewSetupId { get; set; }
  public BrewSetup BrewSetup { get; set; } = null!;
  public string? Name { get; set; }
  public string? Note { get; set; }
}
