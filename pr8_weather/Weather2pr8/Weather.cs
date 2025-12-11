using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather2pr8
{
    public class WeatherData
    {
        public string DateHeader { get; set; }
        public string TimeOfDay { get; set; }
        public string TemperatureRange { get; set; }
        public string WeatherCondition { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
        public string WindDirection { get; set; }
        public string FeelsLike { get; set; }
        public string UvIndex { get; set; }
        public string MagneticField { get; set; }
    }

    public class WaetherForecast
    {
        public ObservableCollection<WeatherData> Items { get; } = new ObservableCollection<WeatherData>();
    }
}
