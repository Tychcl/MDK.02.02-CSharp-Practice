using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SnakeWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow mainWindow;
        public ViewModelUserSettings ViewModelUserSettings = new ViewModelUserSettings();
        public ViewModelGames ViewModelGames = null;
        public ViewModelGames _ViewModelGames = null;
        public List<ViewModelGames> ViewModelGamesList = null;
        public List<ViewModelGames> _ViewModelGamesList = null;
        public static IPAddress remoteIPAddress = IPAddress.Parse("127.0.0.1");
        public static int remotePort = 5001;
        public Thread tRec;
        public Thread tUI;
        public UdpClient receivingUdpClient;
        public Pages.Home Home = new Pages.Home();
        public Pages.Game Game = new Pages.Game();
        //колличество промежуточных кадров
        public int d = 10;
        public DateTime _firstTime = new DateTime();
        public DateTime _lastTime = new DateTime();
        public int _sleep = 1;

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
            OpenPage(Home);
        }
        public void StartReceiver()
        {
            tRec = new Thread(new ThreadStart(Receiver));
            tRec.Start();
        }

        public void StartUI()
        {
            tUI = new Thread(new ThreadStart(UpdUI));
            tUI.Start();
        }
        public void OpenPage(Page PageOpen)
        {
            DoubleAnimation startAnimation = new DoubleAnimation();
            startAnimation.From = 1;
            startAnimation.To = 0;
            startAnimation.Duration = TimeSpan.FromSeconds(0.6);
            startAnimation.Completed += delegate
            {
                frame.Navigate(PageOpen);
                DoubleAnimation endAnimation = new DoubleAnimation();
                startAnimation.From = 0;
                startAnimation.To = 1;
                startAnimation.Duration = TimeSpan.FromSeconds(0.6);
                frame.BeginAnimation(OpacityProperty, endAnimation);
            };
            frame.BeginAnimation(OpacityProperty, startAnimation);
        }

        public void UpdUI()
        {
            try
            {
                Dispatcher.Invoke(() => OpenPage(Game));

                while (true)
                {
                    if (ViewModelGames == null)
                    {
                        Thread.Sleep(16); 
                        continue;
                    }

                    if (ViewModelGames.SnakesPlayer?.GameOver == true)
                    {
                        Dispatcher.Invoke(() => OpenPage(new Pages.GameOver()));
                        break; 
                    }
                    else
                    {
                        Task.Run(() => Game.CreateUI());

                        Thread.Sleep(Math.Max(16, _sleep)); 
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("UpdUI exception: " + ex.Message);
            }
        }

        public void Receiver()
        {
            receivingUdpClient = new UdpClient(int.Parse(ViewModelUserSettings.Port));
            IPEndPoint RemoteIpEndPoint = null;
            _firstTime = DateTime.Now;

            try
            {
                while (true)
                {
                    byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);
                    string returnDate = Encoding.UTF8.GetString(receiveBytes);

                    _lastTime = _firstTime;
                    _firstTime = DateTime.Now;

                    TimeSpan timeDiff = _firstTime - _lastTime;
                    int _s = (int)Math.Max(0, Math.Min(timeDiff.TotalMilliseconds, int.MaxValue));

                    if (_s < 16) // Минимум ~60 FPS
                        continue;

                    _sleep = Math.Min(_s, 1000); // Ограничиваем максимум 1 секундой

                    _ViewModelGames = ViewModelGames;
                    _ViewModelGamesList = ViewModelGamesList;

                    ViewModelGames = JsonConvert.DeserializeObject<ViewModelGames>(returnDate);

                    if (ViewModelGames?.SnakesPlayer?.GameOver != true)
                    {
                        try
                        {
                            receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);
                            returnDate = Encoding.UTF8.GetString(receiveBytes);
                            ViewModelGamesList = JsonConvert.DeserializeObject<List<ViewModelGames>>(returnDate);
                        }
                        catch (Exception innerEx)
                        {
                            Debug.WriteLine($"Error receiving additional data: {innerEx.Message}");
                            ViewModelGamesList = new List<ViewModelGames>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Receiver exception: {ex.Message}");
            }
        }

        public void Send(string datagram)
        {
            UdpClient sender = new UdpClient();
            IPEndPoint endPoint = new IPEndPoint(remoteIPAddress, remotePort);
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(datagram);
                sender.Send(bytes, bytes.Length, endPoint);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Возникло исключение: " + ex.ToString() + "\n " + ex.Message);
            }
            finally
            {
                sender.Close();
            }
        }
        private void EventKeyUp(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(ViewModelUserSettings.IPAddress) && !string.IsNullOrEmpty(ViewModelUserSettings.Port) && (ViewModelGames != null && !ViewModelGames.SnakesPlayer.GameOver))
            {
                if (e.Key == Key.Up)
                    Send($"Up|{JsonConvert.SerializeObject(ViewModelUserSettings)}");
                else if (e.Key == Key.Down)
                    Send($"Down|{JsonConvert.SerializeObject(ViewModelUserSettings)}");
                else if (e.Key == Key.Left)
                    Send($"Left|{JsonConvert.SerializeObject(ViewModelUserSettings)}");
                else if (e.Key == Key.Right)
                    Send($"Right|{JsonConvert.SerializeObject(ViewModelUserSettings)}");
            }
        }
        private void QuitApplication(object sender, System.ComponentModel.CancelEventArgs e)
        {
            receivingUdpClient.Close();
            tRec.Abort();
            tUI.Abort();
        }
    }
}
