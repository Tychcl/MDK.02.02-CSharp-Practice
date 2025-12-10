using Regin.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private string Mes;
        private smtp._message ype;
        Thread t;
        private User? user = null;
        public Verify(string email, smtp._message type, User? u = null)
        {
            InitializeComponent();
            Email= email;
            ype = type;
            user = u;
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
            try
            {
                if (TbLogin.Text.Length == Code.Length && TbLogin.Text == Code)
                {
                    Mes = "Successful";
                    switch (ype)
                    {
                        case smtp._message.change:
                            t = new Thread(() =>
                            {
                                string pas;
                                smtp.send(Email, ype, out pas);
                                using (var con = new Context())
                                {
                                    var user = con.Users.ToList().Find(x => x.Login == Email);
                                    user.Password = pas;
                                    user.Updated = DateTime.Now;
                                    con.SaveChanges();
                                }
                            });
                            t.Start();
                            break;
                        case smtp._message.verify:
                            if (user is not null)
                            {
                                using (var con = new Context())
                                {
                                    con.Users.Add(user);
                                    con.SaveChanges();
                                }
                            }
                            else
                            {
                                Mes = "Successful authorization";
                                if (MainWindow.previous == MainWindow.page.login)
                                {
                                    MessageBoxResult Result = MessageBox.Show("Set pincode for next authorization?", "Pincode", MessageBoxButton.YesNo);
                                    if (Result == MessageBoxResult.Yes)
                                    {
                                        MainWindow.mainWindow.frame.Navigate(new Pages.SetPin(Email));
                                        return;
                                    }
                                }
                            }
                            break;
                    }
                    Back(null, null);
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Ошибка: "+ex);
                Mes = "Error";
                Back(null, null);
            }
        }

        private void Back(object sender, MouseButtonEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.Login(Mes));
        }
    }
}
