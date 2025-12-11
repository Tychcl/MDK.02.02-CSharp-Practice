using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather2pr8.Classes
{
    public class Response
    {
        public List<Forecast> forecasts { get; set; }
    }
    public class Forecast
    {
        public DateTime date { get; set; }

        public List<Hour> hours { get; set; }
    }

    public class Hour
    {
        public string hour { get; set; }
        public int temp { get; set; }
        public string condition { set
            {
                switch (value)
                {
                    case "clear":
                        RUcondition = "ясно";
                        break;
                    case "partly-cloudy":
                        RUcondition = "малооблачно";
                        break;
                    case "cloudy":
                        RUcondition = "облачно с прояснениями";
                        break;
                    case "overcast":
                        RUcondition = "пасмурно";
                        break;
                    case "light-rain":
                        RUcondition = "небольшой дождь";
                        break;
                    case "rain":
                        RUcondition = "дождь";
                        break;
                    case "heavy-rain":
                        RUcondition = "сильный дождь";
                        break;
                    case "showers":
                        RUcondition = "ливень";
                        break;
                    case "wet-snow":
                        RUcondition = "дождь со снегом";
                        break;
                    case "light-snow":
                        RUcondition = "небольшой снег";
                        break;
                    case "snow":
                        RUcondition = "снег";
                        break;
                    case "snow-showers":
                        RUcondition = "снегопад";
                        break;
                    case "hail":
                        RUcondition = "град";
                        break;
                    case "thunderstorm":
                        RUcondition = "гроза";
                        break;
                    case "thunderstorm-with-rain":
                        RUcondition = "дождь с грозой";
                        break;
                    case "thunderstorm-with-hail":
                        RUcondition = "гроза с градом";
                        break;
                }
            }
        }
        public string RUcondition { get; set; }
        public int pressure_mm { get; set; }
        public int humidity { get; set; }
        public string wind_speed { get; set; }
        public string wind_dir { get; set; }
        public int feels_like { get; set; }
        
    }
}
