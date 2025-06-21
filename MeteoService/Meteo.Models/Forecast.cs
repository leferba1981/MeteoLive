namespace Meteo.Models
{
    public class Forecast
    {

        public string Location { get; set; }

        public bool IsForecast { get; set; } // false = historical data, true = forecast data

        public List<WeatherRecord> Records { get; set; }


    }
}