using Backend.Entities.Abstract;
using NetTopologySuite.Geometries;

namespace Backend.Entities.Revision;

public class RoasterRevision : BaseEntity
{
  // revision snapshot
  public string Name { get; set; } = string.Empty;
  public string? Alias { get; set; }
  public string? LocationAddress { get; set; }
  public Point? LocationCoordinates { get; set; }
  public string? WebsiteUrl { get; set; }
  public string? Description { get; set; }

  public ICollection<Bean> Beans { get; set; } = [];

  // revision metadata
  public int RoasterId { get; set; }
  public int EntityRevisionId { get; set; }

  public Roaster Roaster { get; set; } = null!;
  public EntityRevision EntityRevision { get; set; } = null!;
}
