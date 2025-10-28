namespace Backend.Entities;

public class Bean : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public string? Alias { get; set; }
  public bool Decaf { get; set; }
  public int? ElevationMin { get; set; } // in masl (metres above sea level)
  public int? ElevationMax { get; set; } // in masl (metres above sea level)
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
