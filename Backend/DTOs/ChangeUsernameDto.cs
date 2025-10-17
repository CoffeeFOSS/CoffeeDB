using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class ChangeUsernameDto
{
  [Required(ErrorMessage = "Username is required.")]
  [MinLength(3, ErrorMessage = "Username must be at least 3 characters long.")]
  [MaxLength(20, ErrorMessage = "Username must be at most 20 characters long.")]
  public required string NewUsername { get; set; }

  [Required(ErrorMessage = "Password is required.")]
  public required string Password { get; set; }
}
