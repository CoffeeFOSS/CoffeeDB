using Backend.Entities.Abstract;

namespace Backend.Entities;

public class BrewSetting : BaseAuditableEntity
{
  public int Sour { get; set; } // 1-10
  public int Bitter { get; set; } // 1-10
  public bool Recommended { get; set; }
  public float? WaterTemperature { get; set; } // fahrenheit
  public float? WaterVolume { get; set; } // mL or g
  public float? BrewTime { get; set; } // seconds, composite model
  public float? Dose { get; set; } // in grams
  public float? GrindTime { get; set; } // seconds, composite model
  public string? Note { get; set; }

  public int UserId { get; set; }
  public int BrewSetupId { get; set; }
  public int BeanBatchId { get; set; }

  public User User { get; set; } = null!;
  public BrewSetup BrewSetup { get; set; } = null!;
  public BeanBatch BeanBatch { get; set; } = null!;
  public ICollection<BrewGrinderDialSetting> BrewGrinderDialSettings { get; set; } = [];
}
