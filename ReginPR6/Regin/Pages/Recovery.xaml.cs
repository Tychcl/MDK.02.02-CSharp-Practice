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
        private Common common = new Common();
        public Recovery()
        {
            InitializeComponent();
            common.TbLogin = TbLogin;
            common.LNameUser = LNameUser;
            common.IUser = IUser;
            common.OpacityProperty = OpacityProperty;
        }

        private void OpenLogin(object sender, MouseButtonEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.Login());
        }

        private void SetLogin(object sender, RoutedEventArgs e)
        {
            common.SetLogin();
        }
    }
}
