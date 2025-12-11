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
        public MainWindow()
        {
            InitializeComponent();
            http(55.75222f, 37.61556f);
        }

        public async Task http(float lat, float lon)
        {
            panel.Children.Clear();
            Response response = await GetWeather.Get(lat, lon);
            foreach(var el in response.forecasts)
            {
                if(el.hours.Count != 0)
                {
                    panel.Children.Add(new Plate(el));
                }
            }
        }

        private async void search(object sender, RoutedEventArgs e)
        {
            NominatimResult result = await NominatimResult.Get(tb.Text);
            float.TryParse(result.lat.Replace('.', ','), out float lat);
            float.TryParse(result.lon.Replace('.', ','), out float lon);
            await http(lat, lon);
        }
    }
}