using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class CreateRoasterDto
{
  [Required(ErrorMessage = "Name is required.")]
  [MaxLength(100, ErrorMessage = "Name must be at most 100 characters long.")]
  public required string Name { get; set; }

  [MaxLength(200, ErrorMessage = "Alias must be at most 200 characters long.")]
  public string? Alias { get; set; }

  [MaxLength(500, ErrorMessage = "Location Address must be at most 500 characters long.")]
  public string? LocationAddress { get; set; }

  // TODO: Validation annotations
  public double? LocationCoordinateLatitude { get; set; }
  public double? LocationCoordinateLongitude { get; set; }

  [MaxLength(2000, ErrorMessage = "Description must be at most 2000 characters long.")]
  public string? Description { get; set; }

  [MaxLength(300, ErrorMessage = "WebsiteUrl must be at most 300 characters long.")]
  public string? WebsiteUrl { get; set; }
}
