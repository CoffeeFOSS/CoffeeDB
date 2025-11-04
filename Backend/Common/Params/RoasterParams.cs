using System.ComponentModel.DataAnnotations;

namespace Backend.Common.Params;

public class RoasterParams : PaginationParams, IValidatableObject
{
  public string? Name { get; set; }
  public string? Address { get; set; } // need better DB design for location search

  // PostGIS search
  public float? Lat { get; set; }
  public float? Long { get; set; }
  public float? Radius { get; set; }

  public IEnumerable<ValidationResult> Validate(ValidationContext _)
  {
    var searchValues = new bool[] { Lat.HasValue, Long.HasValue, Radius.HasValue };
    var requiredCount = searchValues.Count(b => b);

    if (requiredCount > 0 && requiredCount < 3)
    {
      yield return new ValidationResult(
          "Latitude, Longitude, and Radius must all be provided together for a spatial search.",
          null
      );
    }
  }
}
