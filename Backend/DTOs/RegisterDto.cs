using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class RegisterDto
{
  [Required]
  [MinLength(3), MaxLength(20)]
  public required string Username { get; set; }
  [Required]
  [MinLength(8), MaxLength(64)]
  public required string Password { get; set; }
}
