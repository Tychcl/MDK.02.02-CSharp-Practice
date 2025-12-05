using Regin.Classes;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Regin.Pages
{
    /// <summary>
    /// Логика взаимодействия для Verify.xaml
    /// </summary>
    public partial class Verify : Page
    {
        private string Code;
        private string Email;
        Thread t;
        public Verify(string email)
        {
            InitializeComponent();
            Email= email;
            resend(null,null);
        }

        private void Timer()
        {
            smtp.send(Email, smtp._message.verify, out Code);
            for (int i = 60; i != 0; i--)
            {
                Dispatcher.Invoke(() =>
                {
                    L.Content = $"Can be resend in {i} seconds.";
                });
                Thread.Sleep(1000);
            }
            Dispatcher.Invoke(() =>
            {
                L.Content = $"Can resend.";
                but.IsEnabled = true;
            });
        }

        private void resend(object sender, RoutedEventArgs e)
        {
            but.IsEnabled = false;
            t = new Thread(Timer);
            t.Start();
        }

        private void SetCode(object sender, RoutedEventArgs e)
        {
            if (TbLogin.Text.Length == Code.Length && TbLogin.Text == Code)
            {
                t = new Thread(() =>
                {
                    string pas;
                    smtp.send(Email, smtp._message.change, out pas);
                    using(var con = new Context())
                    {
                        var user = con.Users.ToList().Find(x => x.Login == Email);
                        user.Password = pas;
                        con.SaveChanges();
                    }
                });
                t.Start();
                MessageBox.Show("Password changed");
                Back(null, null);
            }
        }

        private void Back(object sender, MouseButtonEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.Login());
        }
    }
}
