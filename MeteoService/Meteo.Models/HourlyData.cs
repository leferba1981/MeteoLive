using System.Collections.Generic;

namespace Meteo.Models
{
    public class HourlyData
    {
        public List<long> Time { get; set; }
        public List<double> Temperature_2m { get; set; }
        public List<double> ApparentTemperature { get; set; }
        public List<double> Precipitation { get; set; }
        public List<double> WindSpeed_10m { get; set; }
        public List<double> Rain { get; set; }
    }
}