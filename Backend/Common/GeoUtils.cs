using Backend.DTOs;
using NetTopologySuite.Geometries;

namespace Backend.Common;

public static class GeoUtils
{
  public static Point CreatePoint(double latitude, double longitude)
  {
    // 11cm of precision https://en.wikipedia.org/wiki/Decimal_degrees
    double roundedLatitude = Math.Round(latitude, 6, MidpointRounding.AwayFromZero);
    double roundedLongitude = Math.Round(longitude, 6, MidpointRounding.AwayFromZero);

    // The lat/long is intentionally swapped here, Point expects it in that order
    return new Point(roundedLongitude, roundedLatitude) { SRID = 4326 };
  }

  public static CoordinatesDto? ToCoordinatesDto(Point? point)
  {
    if (point == null) return null;

    return new CoordinatesDto
    {
      Latitude = point.Y,   // Y = latitude
      Longitude = point.X   // X = longitude
    };
  }
}
