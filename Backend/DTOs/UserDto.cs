namespace Backend.DTOs;

// used to return token data to the client
public class UserDto
{
  public required int Id { get; set; }
  public required string Username { get; set; }
  public required string Token { get; set; }
}
