using NetTopologySuite.Geometries;

namespace Backend.Common;

public static class GeoUtils
{
  public static Point CreatePoint(double latitude, double longitude)
  {
    return new Point(longitude, latitude) { SRID = 4326 };
  }
}
