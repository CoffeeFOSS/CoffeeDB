namespace Backend.Common.Params;

public class RoasterParams : PaginationParams
{
  public string? Name { get; set; }
  public string? LocationAddress { get; set; } // need better DB design for location search

  // PostGIS search
  public double? CoordinatesLatitude { get; set; }
  public double? CoordinatesLongitude { get; set; }
  public double? LocationWithinKilometers { get; set; }
}
