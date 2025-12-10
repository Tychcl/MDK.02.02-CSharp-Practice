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

namespace Weather2pr8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static HttpClient httpClient = new HttpClient();
        private readonly string url = "https://api.gismeteo.net/v4/weather/forecast/h6?locale=ru-RU&";
        public MainWindow()
        {
            InitializeComponent();
            //MessageBox.Show();
            GetWeather();
        }

        private async Task<List<WeatherData>> GetWeather(string City = "Москва", 
                                                        string lat = "55.7522", string lon = "37.6156")
        {
            List<WeatherData> weather = new List<WeatherData>();
            var uriBuilder = new UriBuilder("https://api.gismeteo.net/v4/weather/forecast/h6");
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            query["locale"] = "ru-RU";
            query["name"] = City;
            query["param2"] = "value2";
            uriBuilder.Query = query.ToString();

            HttpResponseMessage response = await httpClient.GetAsync(uriBuilder.Uri);

            return weather;
        }
    }
}