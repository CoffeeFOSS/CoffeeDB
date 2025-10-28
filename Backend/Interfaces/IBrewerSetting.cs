namespace Backend.Interfaces;

public interface IBrewerSetting
{
  decimal WaterTemperature { get; set; } // fahrenheit
  decimal WaterVolume { get; set; } // grams (or mL)
  decimal BrewTime { get; set; } // seconds
}
