using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class ChangePasswordDto
{
  [Required(ErrorMessage = "Password is required.")]
  public required string CurrentPassword { get; set; }

  [Required(ErrorMessage = "New password is required.")]
  [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
  [MaxLength(64, ErrorMessage = "Password must be at most 64 characters long.")]
  public required string NewPassword { get; set; }

  [Required(ErrorMessage = "Confirm new password is required.")]
  [MinLength(8, ErrorMessage = "Confirm Password must be at least 8 characters long.")]
  [MaxLength(64, ErrorMessage = "Confirm Password must be at most 64 characters long.")]
  public required string ConfirmNewPassword { get; set; }
}
