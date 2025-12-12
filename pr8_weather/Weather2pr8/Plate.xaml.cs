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
        public Plate(Period period)
        {
            InitializeComponent();
            datee.Content = $"{period.date.Day} {month(period.date.Month)}, {period.date.DayOfWeek}";
            dataGrid.ItemsSource = period.periods;
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

    }
}
