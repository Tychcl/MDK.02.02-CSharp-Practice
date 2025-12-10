using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace UC
{
    /// <summary>
    /// Логика взаимодействия для Capture.xaml
    /// </summary>
    public partial class Capture : UserControl
    {
        public delegate void Correct();
        public event Correct HandlerCorrect;
        public event Correct HandlerInCorrect;
        string str = "";
        int width = 280;
        int height = 50;
        public Capture()
        {
            InitializeComponent();
            Create();
        }

        private void Create()
        {
            Input.Text = "";
            Cap.Children.Clear();
            str = "";
            CreateBack();
            Background();
        }
        #region Create()
        void CreateBack()
        {
            Random random = new Random();
            for (int i = 0; i < random.Next(75, 150); i++)
            {
                int back = random.Next(0, 10);
                Label L = new Label()
                {
                    Content = back,
                    FontSize = random.Next(10, 16),
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromArgb(100, (byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255))),
                    Margin = new Thickness(random.Next(0, width - 20), random.Next(0, height - 20), 0, 0)
                };
                Cap.Children.Add(L);
            }
        }

        void Background()
        {
            Random random = new Random();
            for (int i = 0; i < 4; i++)
            {
                int back = random.Next(0, 10);
                Label L = new Label()
                {
                    Content = back,
                    FontSize = 30,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromArgb(255, (byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255))),
                    Margin = new Thickness(width / 2 - 60 + i * 30, random.Next(-10, 10), 0, 0)
                };
                str += back.ToString();
                Cap.Children.Add(L);
            }
        }
        #endregion

        private void Enter(object sender, KeyEventArgs e)
        {
            if(Input.Text.Length == 4)
            {
                if(Input.Text != str)
                {
                    Create();
                    if (HandlerInCorrect is not null)
                    {
                        HandlerInCorrect.Invoke();
                    }
                }
                else if(HandlerCorrect is not null)
                {
                    HandlerCorrect.Invoke();
                }
            }
        }
    }
}
