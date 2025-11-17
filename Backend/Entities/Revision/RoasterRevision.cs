using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Backend.Entities.Revision;

public class RoasterRevision
{
  [DatabaseGenerated(DatabaseGeneratedOption.None)]
  public int Id { get; private set; } // Created to be the same as EntityRevision Id, so this cannot be autoincremented

  // revision snapshot
  public string Name { get; set; } = string.Empty;
  public string? Alias { get; set; }
  public string? LocationAddress { get; set; }
  public Point? LocationCoordinates { get; set; }
  public string? WebsiteUrl { get; set; }
  public string? Description { get; set; }

  public ICollection<Bean> Beans { get; set; } = [];

  // revision metadata
  public int? RoasterId { get; set; }
  public int EntityRevisionId { get; set; }

  public Roaster? Roaster { get; set; } = null!;
  public EntityRevision EntityRevision { get; set; } = null!;

  public RoasterRevision(int id)
  {
    if (id <= 0) throw new ArgumentException("Id must be a positive integer.", nameof(id));
    Id = id;
  }
}
