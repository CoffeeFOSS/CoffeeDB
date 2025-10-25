using Backend.Interfaces;

namespace Backend.Entities;

public class BrewSetting : BaseAuditableEntity, IBrewerSetting
{
  public decimal Dose { get; set; } // in grams
  public int Sour { get; set; } // 1-10
  public int Bitter { get; set; } // 1-10
  public bool Recommended { get; set; }
  public decimal WaterTemperature { get; set; } // celsius
  public decimal WaterVolume { get; set; } // mL or g
  public decimal BrewTime { get; set; } // seconds
  public string? Comments { get; set; }

  public int UserId { get; set; }
  public int BrewSetupId { get; set; }
  public int BeanBatchId { get; set; }

  public User User { get; set; } = null!;
  public BrewSetup BrewSetup { get; set; } = null!;
  public BeanBatch BeanBatch { get; set; } = null!;
  public ICollection<BrewGrinderDialSetting> BrewGrinderDialSettings { get; set; } = [];
}
