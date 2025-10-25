using Backend.Interfaces;

namespace Backend.Entities;

public class BrewSetting : BaseAuditableEntity, IBrewerSetting
{
  public int UserId { get; set; }
  public User User { get; set; } = null!;
  public int BrewSetupId { get; set; }
  public BrewSetup BrewSetup { get; set; } = null!;
  public int BeanBatchId { get; set; }
  public BeanBatch BeanBatch { get; set; } = null!;
  public decimal Dose { get; set; } // in grams
  public int Sour { get; set; } // 1-10
  public int Bitter { get; set; } // 1-10
  public bool Recommended { get; set; }
  public decimal WaterTemperature { get; set; } // celsius
  public decimal WaterVolume { get; set; } // mL or g
  public decimal BrewTime { get; set; } // seconds
  public string? Comments { get; set; }
  public ICollection<BrewGrinderDialSetting> BrewGrinderDialSettings { get; set; } = [];
}
