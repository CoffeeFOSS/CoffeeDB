using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class CreateRoasterRevisionDto
{
  [Required(ErrorMessage = "Name is required.")]
  [MaxLength(100, ErrorMessage = "Name must be at most 100 characters long.")]
  public required string Name { get; set; } = string.Empty;

  [Required(ErrorMessage = "Comment is required.")]
  [MaxLength(300, ErrorMessage = "Comment must be at most 300 characters long.")]
  public required string Comment { get; set; } = string.Empty;

  [MaxLength(200, ErrorMessage = "Alias must be at most 200 characters long.")]
  public string? Alias { get; set; }

  [MaxLength(500, ErrorMessage = "Location Address must be at most 500 characters long.")]
  public string? LocationAddress { get; set; }

  [Range(typeof(double), "-90", "90", ErrorMessage = "Latitude must be between -90 and 90.")]
  public double? LocationCoordinateLatitude { get; set; }

  [Range(typeof(double), "-180", "180", ErrorMessage = "Latitude must be between -90 and 90.")]
  public double? LocationCoordinateLongitude { get; set; }

  [MaxLength(300, ErrorMessage = "WebsiteUrl must be at most 300 characters long.")]
  public string? WebsiteUrl { get; set; }

  [MaxLength(2000, ErrorMessage = "Description must be at most 2000 characters long.")]
  public string? Description { get; set; }
}
