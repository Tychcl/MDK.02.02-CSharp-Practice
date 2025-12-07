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
using System.Xml.Serialization;

namespace Regin.Pages
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        private Common common = new Common();
        private bool cap = false;
        public Login(string message = "")
        {
            InitializeComponent();
            MainWindow.previous = MainWindow.page.login;
            common.TbLogin = TbLogin;
            common.LNameUser = LNameUser;
            common.IUser = IUser;
            common.OpacityProperty = OpacityProperty;
            Captur.HandlerCorrect += delegate { cap = true; Captur.IsEnabled = false; };
            Captur.HandlerInCorrect += delegate { cap = false; };
            Common.SetNotification(LNameUser ,message, message == "Error" ? Brushes.Red : Brushes.Green);
        }

        private void SetPassword(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if(common.user is null)
                {
                    Common.SetNotification(LNameUser, "User not exists", Brushes.Red);
                }
                else if(common.user.Password != TbPassword.Password & common.user.PinCode.ToString() != TbPassword.Password)
                {
                    Common.SetNotification(LNameUser, "Wrong password or pincode.", Brushes.Red);
                }
                else if (!cap)
                {
                    Common.SetNotification(LNameUser, "Confirm capture.", Brushes.Red);
                }
                else
                {
                    MainWindow.mainWindow.frame.Navigate(new Pages.Verify(TbLogin.Text, smtp._message.verify));
                }
            }
            else
            {
                Common.SetNotification(LNameUser, "", Brushes.Green);
            }
        }

        private void OpenRegin(object sender, MouseButtonEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.Regin());
        }

        private void RecoveryPassword(object sender, MouseButtonEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.Recovery());
        }

        private void SetLogin(object sender, RoutedEventArgs e)
        {
            common.SetLogin();
        }
    }
}
