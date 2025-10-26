namespace Backend.Entities;

public class Bean : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public string? Aliases { get; set; }
  public bool Decaf { get; set; }
  public int? Elevation { get; set; } // in masl (metres above sea level)
  public string? Roast { get; set; }
  public string? Type { get; set; } // arabica...
  public string? Region { get; set; }
  public string? Farm { get; set; }
  public string? WetMill { get; set; }
  public string? Varietal { get; set; }
  public string? Producer { get; set; }
  public string? Importer { get; set; }
  public string? Process { get; set; }
  public string? FlavorProfile { get; set; }
  public int? ReleaseDate { get; set; } // YYYYMMDD, unknown MMDD use 0000

  public int? RoasterId { get; set; }

  public Roaster? Roaster { get; set; } = null!;
  public ICollection<BeanBatch> BeanBatches { get; set; } = [];
  public ICollection<BrewSetup> BrewSetups { get; set; } = [];
}
