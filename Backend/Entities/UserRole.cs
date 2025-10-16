using Microsoft.AspNetCore.Identity;

namespace Backend.Entities;

// Join table for User and Role
public class UserRole : IdentityUserRole<int>
{
  public User User { get; set; } = null!;
  public Role Role { get; set; } = null!;
}
