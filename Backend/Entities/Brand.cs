using Backend.Entities.Abstract;

namespace Backend.Entities;

public class Brand : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public string? Alias { get; set; }
  public string? Description { get; set; }
  public string? Country { get; set; }

  public ICollection<Brewer> Brewers { get; set; } = [];
  public ICollection<Grinder> Grinders { get; set; } = [];
}
