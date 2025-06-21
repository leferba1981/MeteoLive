using System;

namespace Meteo.Models
{

    public class WeatherRecord
    {

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int Altitude { get; set; }

        public double Temperature { get; set; }

        public double Pressure { get; set; }

        public int Precipitation { get; set; }

        public double Humidity { get; set; }

        public double Radiation { get; set; }

        public double WindSpeed { get; set; }

        public double WindDirection { get; set; }
        
        public DateTime DateTime { get; set; }
    }
}