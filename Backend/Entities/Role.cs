using Microsoft.AspNetCore.Identity;

namespace Backend.Entities;

public class Role : IdentityRole<int>
{
  public ICollection<UserRole> UserRoles { get; set; } = [];
}
