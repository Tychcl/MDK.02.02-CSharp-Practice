using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Weather2pr8.Classes;

namespace Weather2pr8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int limit = 5;
        public MainWindow()
        {
            InitializeComponent();
            //http(55.75222f, 37.61556f);
        }

        public async Task http(float lat, float lon, string city = "")
        {
            Response response = await GetWeather.Get(lat, lon);
            List<Period> p = new List<Period>();
            foreach(var el in response.forecasts)
            {
                if(el.hours.Count != 0)
                {
                    var list = GroupHoursIntoPeriods(el.hours);
                    Period period = new Period(el.date, list);
                    period.city = city;
                    p.Add(period);
                    panel.Children.Add(new Plate(period));
                }
            }
            using (var con = new Context())
            {
                con.Requests.Add(new DBPeriodSummary(city,p));
                con.SaveChanges();
            }
        }

        private async void search(object sender, RoutedEventArgs e)
        {
            panel.Children.Clear();
            using (var con = new Context())
            {
                DBPeriodSummary f = con.Requests.ToList().Find(x => x.City == tb.Text && x.RequestDate == DateTime.Now.Date);
                if(f is not null)
                {
                    var list = JsonConvert.DeserializeObject<List<Period>>(f.PeriodJsonList);
                    foreach(var el in list)
                    {
                        panel.Children.Add(new Plate(el));
                    }
                    return;
                }
                int c = con.Requests.ToList().Count(x => x.RequestDate == DateTime.Now.Date);
                if(c >= limit)
                {
                    MessageBox.Show($"Превышен лимит запросов: {limit}");
                    return;
                }

            }
            NominatimResult result = await NominatimResult.Get(tb.Text);
            float.TryParse(result.lat.Replace('.', ','), out float lat);
            float.TryParse(result.lon.Replace('.', ','), out float lon);
            await http(lat, lon, tb.Text);
        }

        public List<PeriodSummary> GroupHoursIntoPeriods(List<Hour> hours)
        {
            var periods = new List<PeriodSummary>();

            var periodDefinitions = new[]
            {
                new { Name = "Утро", Start = 6, End = 11 },   // 6-11 часов
                new { Name = "День", Start = 12, End = 17 },  // 12-17 часов
                new { Name = "Вечер", Start = 18, End = 23 }, // 18-23 часов
                new { Name = "Ночь", Start = 0, End = 5 }     // 0-5 часов
            };

            foreach (var periodDef in periodDefinitions)
            {
                var periodHours = hours
                    .Where(h =>
                    {
                        // Парсим час из строки "HH:00"
                        if (int.TryParse(h.hour.Split(':')[0], out int hourValue))
                        {
                            return hourValue >= periodDef.Start && hourValue <= periodDef.End;
                        }
                        return false;
                    })
                    .ToList();

                if (periodHours.Any())
                {
                    var summary = new PeriodSummary
                    {
                        hour = periodDef.Name,
                        temp = $"{periodHours.Min(h => h.temp)} ... {periodHours.Max(h => h.temp)}",
                        pressure_mm = (int)periodHours.Average(h => h.pressure_mm),
                        humidity = (int)periodHours.Average(h => h.humidity),
                        feels_like = $"{periodHours.Min(h => h.feels_like)} ... {periodHours.Max(h => h.feels_like)}",
                        condition = periodHours
                            .GroupBy(h => h.RUcondition)
                            .OrderByDescending(g => g.Count())
                            .First().Key,
                        wind_dir = periodHours
                            .GroupBy(h => h.wind_dir)
                            .OrderByDescending(g => g.Count())
                            .First().Key
                    };

                    // Обработка скорости ветра (предполагаем формат "X м/с")
                    var windSpeeds = periodHours
                        .Select(h =>
                        {
                            var cleanSpeed = h.wind_speed.Replace(" м/с", "").Replace(" ", "").Replace(".", ",");
                            return double.Parse(cleanSpeed);
                        })
                        .ToList();

                    summary.wind_speed = $"{Math.Round(windSpeeds.Average(), 1)} м/с";

                    periods.Add(summary);
                }
            }

            return periods;
        }
    }
}