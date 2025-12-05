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
        public Login()
        {
            InitializeComponent();
            common.TbLogin = TbLogin;
            common.LNameUser = LNameUser;
            common.IUser = IUser;
            common.OpacityProperty = OpacityProperty;
            Capture.HandlerCorrect += delegate { cap = true; Capture.IsEnabled = false; };
            Capture.HandlerInCorrect += delegate { cap = false; };
        }

        private void SetPassword(object sender, KeyEventArgs e)
        {

        }

        private void OpenRegin(object sender, MouseButtonEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.Login());
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
