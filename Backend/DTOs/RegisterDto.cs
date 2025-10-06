using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class RegisterDto
{
  [Required(ErrorMessage = "Username is required.")]
  [MinLength(3, ErrorMessage = "Username must be at least 3 characters long.")]
  [MaxLength(20, ErrorMessage = "Username must be at most 20 characters long.")]
  public required string Username { get; set; }

  [Required(ErrorMessage = "Password is required.")]
  [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
  [MaxLength(64, ErrorMessage = "Password must be at most 64 characters long.")]
  public required string Password { get; set; }

  [Required(ErrorMessage = "Confirm Password is required.")]
  [MinLength(8, ErrorMessage = "Confirm Password must be at least 8 characters long.")]
  [MaxLength(64, ErrorMessage = "Confirm Password must be at most 64 characters long.")]
  public required string ConfirmPassword { get; set; }
}
