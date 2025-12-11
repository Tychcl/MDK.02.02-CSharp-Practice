using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Логика взаимодействия для Plate.xaml
    /// </summary>
    public partial class Plate : UserControl
    {
        public Plate(Forecast forecast)
        {
            InitializeComponent();
            datee.Content = $"{forecast.date.Day} {month(forecast.date.Month)}, {forecast.date.DayOfWeek}";
            dataGrid.ItemsSource = GroupHoursIntoPeriods(forecast.hours);
        }

        private string month(int m)
        {
            switch (m)
            {
                case 1:
                    return "Январь";
                case 2:
                    return "Февраль";
                case 3:
                    return "Март";
                case 4:
                    return "Апрель";
                case 5:
                    return "Май";
                case 6:
                    return "Июнь";
                case 7:
                    return "Июль";
                case 8:
                    return "Август";
                case 9:
                    return "Сентябрь";
                case 10:
                    return "Октябрь";
                case 11:
                    return "Ноябрь";
                case 12:
                    return "Декабрь";
                default:
                    return "Неверный номер месяца";
            }
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
