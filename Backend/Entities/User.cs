namespace Backend.Entities;

public class User
{
  public int Id { get; set; }
  public required string UserName { get; set; } // this casing is for .NET identity later
  public byte[] PasswordHash { get; set; } = [];
  public byte[] PasswordSalt { get; set; } = [];
}
