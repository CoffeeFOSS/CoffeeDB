using Backend.Entities.Abstract;
using Backend.Entities.Revision;
using NetTopologySuite.Geometries;

namespace Backend.Entities;

public class Roaster : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public string? Alias { get; set; }
  public string? LocationAddress { get; set; }
  public Point? LocationCoordinates { get; set; }
  public string? WebsiteUrl { get; set; }
  public string? Description { get; set; }

  public ICollection<Bean> Beans { get; set; } = [];
  public ICollection<RoasterRevision> Revisions { get; set; } = [];
}
