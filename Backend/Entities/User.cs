namespace Backend.Entities;

public class User
{
  public int Id { get; set; }
  public required string Username { get; set; }
  public byte[] PasswordHash { get; set; } = [];
  public byte[] PasswordSalt { get; set; } = [];
}
