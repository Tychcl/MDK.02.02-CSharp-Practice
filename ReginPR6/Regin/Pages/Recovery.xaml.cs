using Regin.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using static Regin.Elements.Capture;

namespace Regin.Pages
{
    /// <summary>
    /// Логика взаимодействия для Recovey.xaml
    /// </summary>
    public partial class Recovery : Page
    {
        private bool correct = false;
        private string OldLogin;
        private Context con = new Context();
        public Recovery()
        {
            InitializeComponent();
        }

        private void OpenLogin(object sender, MouseButtonEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.Login());
        }


        private void SetNotification(string mes, SolidColorBrush color)
        {
            LNameUser.Content = mes;
            LNameUser.Foreground = color;
        }

        private void SetLogin(object sender, RoutedEventArgs e)
        {
            string login = TbLogin.Text;
            if (Regex.IsMatch(login, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                var user = con.Users.ToList().Find(x => x.Login == login);
                if (user is not null)
                {
                    correct = true;
                    CorrectLogin(user);
                }
                else
                {
                    SetNotification("User not found", Brushes.Red);
                }
            }
            else if (!correct)
            {
                SetNotification("Login is incorrect", Brushes.Red);
            }
            else
            {
                InCorrectLogin();
            }
        }

        public void InCorrectLogin()
        {
            if (LNameUser.Content != "")
            {
                LNameUser.Content = "";
                DoubleAnimation StartAnimation = new DoubleAnimation();
                StartAnimation.From = 1;
                StartAnimation.To = 0;
                StartAnimation.Duration = TimeSpan.FromSeconds(0.6);
                StartAnimation.Completed += delegate
                {
                    IUser.Source = new BitmapImage(new Uri("pack://application:,,,/Images/ic-user.jpg"));
                    DoubleAnimation EndAnimation = new DoubleAnimation();
                    EndAnimation.From = 0;
                    EndAnimation.To = 1;
                    EndAnimation.Duration = TimeSpan.FromSeconds(1.2);
                    IUser.BeginAnimation(OpacityProperty, EndAnimation);
                };
                IUser.BeginAnimation(OpacityProperty, StartAnimation);
            }

            if (TbLogin.Text.Length > 0)
                SetNotification("Login is incorrect", Brushes.Red);
            correct = false;
        }

        public void CorrectLogin(User User)
        {
            if (OldLogin != TbLogin.Text)
            {
                SetNotification("Hi, " + User.Name, Brushes.Black);

                try
                {
                    BitmapImage bling = new BitmapImage();
                    MemoryStream ms = new MemoryStream(User.Image);
                    bling.BeginInit();
                    bling.StreamSource = ms;
                    bling.EndInit();

                    ImageSource imgSrc = bling;
                    DoubleAnimation StartAnimation = new DoubleAnimation();
                    StartAnimation.From = 1;
                    StartAnimation.To = 0;
                    StartAnimation.Duration = TimeSpan.FromSeconds(0.6);
                    StartAnimation.Completed += delegate
                    {
                        IUser.Source = imgSrc;
                        DoubleAnimation EndAnimation = new DoubleAnimation();
                        EndAnimation.From = 0;
                        EndAnimation.To = 1;
                        EndAnimation.Duration = TimeSpan.FromSeconds(1.2);
                        IUser.BeginAnimation(Image.OpacityProperty, EndAnimation);
                    };

                    IUser.BeginAnimation(Image.OpacityProperty, StartAnimation);
                }
                catch (Exception exp)
                {
                    Debug.WriteLine(exp.Message);
                }
                ;

                OldLogin = TbLogin.Text;
            }
        }
    }
}
