namespace Backend.Entities;

public class BrewMethod : BaseEntity
{
  public string Name { get; set; } = string.Empty;

  public ICollection<Brewer> Brewers { get; set; } = [];
}
