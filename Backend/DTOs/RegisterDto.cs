using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class RegisterDto
{
  [Required]
  // [MaxLength(32)]
  public required string Username { get; set; }
  [Required]
  public required string Password { get; set; }
}
