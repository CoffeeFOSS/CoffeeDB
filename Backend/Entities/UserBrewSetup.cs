namespace Backend.Entities;

public class UserBrewSetup
{
  public int UserId { get; set; }
  public User User { get; set; } = null!;
  public int BrewSetupId { get; set; }
  public BrewSetup BrewSetup { get; set; } = null!;
  public string? Name { get; set; }
  public string? Notes { get; set; }
}
