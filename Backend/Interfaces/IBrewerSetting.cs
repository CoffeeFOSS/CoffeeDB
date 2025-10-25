namespace Backend.Interfaces;

public interface IBrewerSetting
{
  decimal WaterTemperature { get; set; } // celsius
  decimal WaterVolume { get; set; } // grams (or mL)
  decimal BrewTime { get; set; } // seconds
}
