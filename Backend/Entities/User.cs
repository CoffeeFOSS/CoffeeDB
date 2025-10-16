using Microsoft.AspNetCore.Identity;

namespace Backend.Entities;

public class User : IdentityUser<int>
{
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime LastActive { get; set; } = DateTime.UtcNow; // TODO: Make sure this is used, or remove it
  public DateTime? LastUsernameChangedAt { get; set; }
  public ICollection<UserRole> UserRoles { get; set; } = [];
}
