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

namespace Regin.Pages
{
    /// <summary>
    /// Логика взаимодействия для SetPin.xaml
    /// </summary>
    public partial class SetPin : Page
    {
        private string em;
        public SetPin(string email)
        {
            InitializeComponent();
            em = email;
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            int pin;
            if(TbLogin.Text.Length == 4 && int.TryParse(TbLogin.Text, out pin))
            {
                using (var con = new Context())
                {
                    var u = con.Users.ToList().Find(x => x.Login == em);
                    if(u is not null)
                    {
                        u.PinCode = pin;
                        u.Updated = DateTime.Now;
                        con.SaveChanges();
                        MainWindow.mainWindow.frame.Navigate(new Pages.Login("Pincode confirmed"));
                    }
                }
            }
        }

        private void Back(object sender, MouseButtonEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.Login());
        }
    }
}
