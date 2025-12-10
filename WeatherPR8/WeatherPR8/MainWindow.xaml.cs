using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace WeatherPR8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public partial class MainWindow : Window, INotifyPropertyChanged
        {
            private ObservableCollection<WeatherData> _weatherItems;
            private ObservableCollection<string> _cities;
            private string _selectedCity;
            private DateTime _lastUpdate;
            private bool _isLoading;
            private DateTime _currentTime;

            public ObservableCollection<WeatherData> WeatherItems
            {
                get => _weatherItems;
                set
                {
                    _weatherItems = value;
                    OnPropertyChanged(nameof(WeatherItems));
                }
            }

            public ObservableCollection<string> Cities
            {
                get => _cities;
                set
                {
                    _cities = value;
                    OnPropertyChanged(nameof(Cities));
                }
            }

            public string SelectedCity
            {
                get => _selectedCity;
                set
                {
                    _selectedCity = value;
                    OnPropertyChanged(nameof(SelectedCity));
                }
            }

            public DateTime LastUpdate
            {
                get => _lastUpdate;
                set
                {
                    _lastUpdate = value;
                    OnPropertyChanged(nameof(LastUpdate));
                }
            }

            public bool IsLoading
            {
                get => _isLoading;
                set
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                    OnPropertyChanged(nameof(IsNotLoading));
                }
            }

            public bool IsNotLoading => !IsLoading;

            public DateTime CurrentTime
            {
                get => _currentTime;
                set
                {
                    _currentTime = value;
                    OnPropertyChanged(nameof(CurrentTime));
                }
            }

            public MainWindow()
            {
                InitializeComponent();
                DataContext = this;

                // Инициализация данных
                InitializeData();

                // Запуск таймера для отображения текущего времени
                var timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += (s, e) => CurrentTime = DateTime.Now;
                timer.Start();

                Loaded += MainWindow_Loaded;
            }

            private void InitializeData()
            {
                WeatherItems = new ObservableCollection<WeatherData>();
                Cities = new ObservableCollection<string>
            {
                "Москва",
                "Санкт-Петербург",
                "Новосибирск",
                "Екатеринбург",
                "Казань",
                "Нижний Новгород",
                "Челябинск",
                "Самара",
                "Омск",
                "Ростов-на-Дону",
                "Уфа",
                "Красноярск",
                "Воронеж",
                "Пермь",
                "Волгоград"
            };

                SelectedCity = Cities.First();

                // Установите ваш API ключ для тестирования (по умолчанию тестовый)
                ApiKeyBox.Password = "demo_key_gismeteo";
            }

            private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
            {
                // Установка цветовой темы
                var paletteHelper = new PaletteHelper();
                var theme = paletteHelper.GetTheme();
                theme.SetPrimaryColor(Color.FromRgb(33, 150, 243)); // Синий цвет
                paletteHelper.SetTheme(theme);

                // Автоматическая загрузка погоды при запуске
                await GetWeatherData();
            }

            private async void GetWeatherButton_Click(object sender, RoutedEventArgs e)
            {
                await GetWeatherData();
            }

            private async void RefreshButton_Click(object sender, RoutedEventArgs e)
            {
                await GetWeatherData();
            }

            private async Task GetWeatherData()
            {
                if (string.IsNullOrEmpty(SelectedCity))
                {
                    UpdateStatus("Пожалуйста, выберите город");
                    return;
                }

                IsLoading = true;
                UpdateStatus("Загрузка данных о погоде...");

                try
                {
                    // Очищаем предыдущие данные
                    WeatherItems.Clear();

                    // Для демонстрации используем тестовые данные
                    // В реальном приложении здесь должен быть запрос к API Gismeteo
                    await LoadTestData();

                    LastUpdate = DateTime.Now;
                    UpdateStatus($"Данные успешно загружены для города {SelectedCity}");
                }
                catch (Exception ex)
                {
                    UpdateStatus($"Ошибка: {ex.Message}");
                }
                finally
                {
                    IsLoading = false;
                }
            }

            private async Task LoadTestData()
            {
                // Имитация загрузки данных
                await Task.Delay(1500);

                var random = new Random();
                var now = DateTime.Now;

                // Генерируем данные на 5 дней
                for (int i = 0; i < 5; i++)
                {
                    var date = now.AddDays(i);

                    // Генерируем 3 записи в день (утро, день, вечер)
                    for (int j = 0; j < 3; j++)
                    {
                        var time = date.AddHours(8 + j * 4); // 8:00, 12:00, 16:00

                        var temperature = random.Next(-10, 30);
                        var weatherType = GetWeatherType(random);

                        var weather = new WeatherData
                        {
                            DateTime = time.ToString("dd.MM.yyyy HH:mm"),
                            Temperature = temperature,
                            Pressure = 740 + random.Next(-10, 10),
                            Humidity = 40 + random.Next(0, 50),
                            Description = GetWeatherDescription(weatherType),
                            WeatherIcon = GetWeatherIcon(weatherType)
                        };

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            WeatherItems.Add(weather);
                        });
                    }
                }
            }

            private string GetWeatherType(Random random)
            {
                string[] types = { "sunny", "cloudy", "rainy", "snowy", "stormy", "foggy" };
                return types[random.Next(types.Length)];
            }

            private string GetWeatherDescription(string weatherType)
            {
                return weatherType switch
                {
                    "sunny" => "Солнечно",
                    "cloudy" => "Облачно",
                    "rainy" => "Дождь",
                    "snowy" => "Снег",
                    "stormy" => "Гроза",
                    "foggy" => "Туман",
                    _ => "Переменная облачность"
                };
            }

            private PackIconKind GetWeatherIcon(string weatherType)
            {
                return weatherType switch
                {
                    "sunny" => PackIconKind.WeatherSunny,
                    "cloudy" => PackIconKind.WeatherCloudy,
                    "rainy" => PackIconKind.WeatherRainy,
                    "snowy" => PackIconKind.WeatherSnowy,
                    "stormy" => PackIconKind.WeatherLightningRainy,
                    "foggy" => PackIconKind.WeatherFog,
                    _ => PackIconKind.WeatherPartlyCloudy
                };
            }

            private void UpdateStatus(string message)
            {
                StatusText.Text = message;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class WeatherData
        {
            public string DateTime { get; set; }
            public double Temperature { get; set; }
            public int Pressure { get; set; }
            public int Humidity { get; set; }
            public string Description { get; set; }
            public PackIconKind WeatherIcon { get; set; }
        }

        // Конвертер для цвета температуры
        public class TemperatureToColorConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (value is double temperature)
                {
                    // Холодно: синий, тепло: оранжевый, горячо: красный
                    if (temperature < 0)
                        return new SolidColorBrush(Color.FromRgb(100, 180, 255)); // Голубой
                    else if (temperature < 10)
                        return new SolidColorBrush(Color.FromRgb(150, 200, 255)); // Светло-голубой
                    else if (temperature < 20)
                        return new SolidColorBrush(Color.FromRgb(255, 200, 100)); // Светло-оранжевый
                    else
                        return new SolidColorBrush(Color.FromRgb(255, 100, 100)); // Красный
                }

                return new SolidColorBrush(Color.FromRgb(100, 180, 255));
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}