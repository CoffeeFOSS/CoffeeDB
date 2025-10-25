using Microsoft.AspNetCore.Identity;

namespace Backend.Entities;

public class User : IdentityUser<int>
{
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  public int? UpdatedById { get; set; }
  public User? UpdatedBy { get; set; }
  public DateTime? UsernameUpdatedAt { get; set; }
  public ICollection<UserRole> UserRoles { get; set; } = [];
  public ICollection<BeanBatch> BeanBatches { get; set; } = [];
  public ICollection<UserBrewSetup> UserBrewSetups { get; set; } = [];
  public ICollection<BrewSetting> BrewSettings { get; set; } = [];

}
