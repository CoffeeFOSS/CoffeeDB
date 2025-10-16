namespace Backend.DTOs;

public class UserWithRolesDto
{
  public int Id { get; set; }
  public string? Username { get; set; }
  public List<string> Roles { get; set; } = [];
}
