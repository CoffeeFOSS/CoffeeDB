using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class UpdateRoasterDto : IValidatableObject
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

  [Range(typeof(float), "-90", "90", ErrorMessage = "Latitude must be between -90 and 90.")]
  public float? LocationCoordinateLatitude { get; set; }

  [Range(typeof(float), "-180", "180", ErrorMessage = "Latitude must be between -90 and 90.")]
  public float? LocationCoordinateLongitude { get; set; }

  [MaxLength(300, ErrorMessage = "WebsiteUrl must be at most 300 characters long.")]
  public string? WebsiteUrl { get; set; }

  [MaxLength(2000, ErrorMessage = "Description must be at most 2000 characters long.")]
  public string? Description { get; set; }


  /// <summary>
  /// Custom validation to ensure at least one field is provided.
  /// </summary>
  public IEnumerable<ValidationResult> Validate(ValidationContext _)
  {
    if (string.IsNullOrWhiteSpace(Name) &&
        string.IsNullOrWhiteSpace(Alias) &&
        string.IsNullOrWhiteSpace(LocationAddress) &&
        string.IsNullOrWhiteSpace(WebsiteUrl) &&
        string.IsNullOrWhiteSpace(Description))
    {
      yield return new ValidationResult(
        "At least one field (Name, Alias, LocationAddress, WebsiteUrl, or Description) must be provided for the update.",
        null
      );
    }
  }
}
