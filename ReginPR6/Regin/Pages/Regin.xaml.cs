using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing.Imaging;
using System.Drawing;
using Regin.Classes;

namespace Regin.Pages
{
    /// <summary>
    /// Логика взаимодействия для Regin.xaml
    /// </summary>
    public partial class Regin : Page
    {
        private bool login = false;
        private bool password = false;
        private byte[]? image = null;

        public Regin()
        {
            InitializeComponent();
        }

        private void SetLogin(object sender, RoutedEventArgs e)
        {
            if (!Regex.IsMatch(TbLogin.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                Common.SetNotification(LNameUser, "Wrong login type", System.Windows.Media.Brushes.Red);
                login = false;
            }
            else
            {
                login = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!login)
            {
                Common.SetNotification(LNameUser, "Incorrect login", System.Windows.Media.Brushes.Red);
                return;
            }
            if (!password)
            {
                Common.SetNotification(LNameUser, "Passwords not equals.", System.Windows.Media.Brushes.Red);
                return;
            }
            if (image is null)
            {
                Common.SetNotification(LNameUser, "Select Image", System.Windows.Media.Brushes.Red);
                return;
            }
            User user = new User(TbLogin.Text, TbPassword.Password, TbName.Text, image);
            MainWindow.mainWindow.frame.Navigate(new Pages.Verify(TbLogin.Text, smtp._message.verify, user));
        }

        private void SetPassword(object sender, KeyEventArgs e)
        {
            if(TbPassword.Password != TbConfirmPassword.Password)
            {
                Common.SetNotification(LNameUser, "Password not equals.", System.Windows.Media.Brushes.Red);
                password = false;
            }
            else
            {
                password = true;
            }
        }

        private void IUser_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter += "Images (*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG";
            if(OFD.ShowDialog() == true)
            {
                Bitmap bitmap1 = new Bitmap(OFD.FileName); 
                Graphics Gr1 = Graphics.FromImage(bitmap1);
                Bitmap bitmap2 = new Bitmap(256, 256, Gr1);
                Graphics Gr2 = Graphics.FromImage(bitmap2);
                System.Drawing.Rectangle compressionRectangle = new System.Drawing.Rectangle
                  (0, 0, 256, 256);
                Gr2.DrawImage(bitmap1, compressionRectangle);
                ImageConverter converter = new ImageConverter();
                image = (byte[])converter.ConvertTo(bitmap2, typeof(byte[]));
                IUser.Source = (BitmapImage)converter.ConvertTo(bitmap2, typeof(BitmapImage));
            }
        }
    }
}
