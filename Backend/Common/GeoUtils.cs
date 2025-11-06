using Backend.DTOs;
using NetTopologySuite.Geometries;

namespace Backend.Common;

public static class GeoUtils
{
  public static Point CreatePoint(double latitude, double longitude)
  {
    // The lat/long is intentionally swapped here, Point expects it in that order
    return new Point(longitude, latitude) { SRID = 4326 };
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
