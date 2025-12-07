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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Regin.Classes;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Regin.Pages
{
    /// <summary>
    /// Логика взаимодействия для Pincode.xaml
    /// </summary>
    public partial class Pincode : Page
    {
        private Common common = new Common();
        public Pincode()
        {
            InitializeComponent();
            common.TbLogin = TbLogin;
            common.LNameUser = mess;
            common.IUser = IUser;
            common.OpacityProperty = OpacityProperty;
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            int pin;
            if (common.user is not null && TbCode.Text.Length == 4 && int.TryParse(TbCode.Text, out pin))
            {
                if(common.user.PinCode == pin)
                {
                    MainWindow.mainWindow.frame.Navigate(new Pages.Login("Successful authorization"));
                }
            }
        }

        private void SetLogin(object sender, RoutedEventArgs e)
        {
            common.SetLogin();
        }

        private void Back(object sender, MouseButtonEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.Login());
        }
    }
}
