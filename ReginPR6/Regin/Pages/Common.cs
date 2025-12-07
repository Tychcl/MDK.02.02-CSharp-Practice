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
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Regin.Classes;

namespace Regin.Pages
{
    public class Common
    {
        public User? user { get; private set; }
        public bool correct = false;
        private Context con = new Context();
        public Label LNameUser { get; set; }
        public Image IUser { get; set; }
        public TextBox TbLogin { get; set; }
        public DependencyProperty OpacityProperty { get; set; }
        private string OldLogin;

        public static void SetNotification(Label label, string mes, SolidColorBrush color)
        {
            label.Content = mes;
            label.Foreground = color;
        }

        public void SetLogin()
        {
            string login = TbLogin.Text;
            if (Regex.IsMatch(login, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                var user = con.Users.ToList().Find(x => x.Login == login);
                if (user is not null)
                {
                    correct = true;
                    this.user = user;
                    CorrectLogin(user);
                }
                else
                {
                    this.user = null;
                    SetNotification(LNameUser ,"User not found", Brushes.Red);
                }
            }
            else if (!correct)
            {
                SetNotification(LNameUser, "Login is incorrect", Brushes.Red);
            }
            else
            {
                InCorrectLogin();
            }
        }

        private void InCorrectLogin()
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
                SetNotification(LNameUser, "Login is incorrect", Brushes.Red);
            correct = false;
        }

        private void CorrectLogin(User User)
        {
            if (OldLogin != TbLogin.Text)
            {
                SetNotification(LNameUser, "Hi, " + User.Name, Brushes.Black);

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
                };

                OldLogin = TbLogin.Text;
            }
        }

    }
}
