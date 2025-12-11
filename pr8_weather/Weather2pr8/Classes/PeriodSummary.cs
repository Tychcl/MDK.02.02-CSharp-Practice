using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather2pr8.Classes
{
    public class PeriodSummary
    {
        public string hour { get; set; } 
        public string temp { get; set; } 
        public string condition { get; set; } 
        public int pressure_mm { get; set; } 
        public int humidity { get; set; } 
        public string wind_speed { get; set; }
        public string wind_dir { get; set; } 
        public string feels_like { get; set; } 
    }
}
