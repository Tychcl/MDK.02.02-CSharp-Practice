using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web;

namespace Weather2pr8.Classes
{
    public class Period
    {
        public string city {  get; set; }
        public DateTime date { get; set; }

        public List<PeriodSummary> periods { get; set; }

        public Period() { }
        public Period(DateTime date, List<PeriodSummary> p)
        {
            this.date = date;
            this.periods = p;
        }
        
    }

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

    public class DBPeriodSummary
    {
        public int Id { get; set; }
        public DateTime RequestDate { get; set; }
        public string City { get; set; }
        public string PeriodJsonList { get; set; }

        public DBPeriodSummary() { }

        public DBPeriodSummary(string city, List<Period> p)
        {
            RequestDate = DateTime.Now.Date;
            City = city;
            PeriodJsonList = JsonConvert.SerializeObject(p);
        }
    }
}
