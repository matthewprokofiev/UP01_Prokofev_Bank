using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace UP01_Prokofev_Bank.Pages
{
    /// <summary>
    /// Interaction logic for AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        public static string GetHash(String password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        private void ShowMessage(string text, string caption = "Сообщение")
        {
            MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private Users Authenticate(string login, string password)
        {
            string hashedPassword = GetHash(password);
            using (var db = new Entities())
            {
                return db.Users.AsNoTracking()
                    .FirstOrDefault(u => u.Login == login && u.Password == hashedPassword);
            }
        }

        private void ButtonEnter_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxLogin.Text) || string.IsNullOrEmpty(PasswordBox.Password))
            {
                ShowMessage("Введите логин и пароль");
                return;
            }

            Users user = Authenticate(TextBoxLogin.Text, PasswordBox.Password);

            if (user == null)
            {
                ShowMessage("Пользователь с такими данными не найден!");
                return;
            }
            else
            {
                ShowMessage("Пользователь успешно найден!");
                switch (user.Role)
                {
                    case "User":
                        NavigationService?.Navigate(new UserPage());
                        break;
                    case "Admin":
                        NavigationService?.Navigate(new AdminPage());
                        break;
                }
            }
        }

        private void ButtonReg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RegPage());
        }

        private void ButtonChangePassword_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ChangePassPage());
        }

        private void txtHintLogin_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBoxLogin.Focus();
        }

        private void txtHintPass_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PasswordBox.Focus();
        }

        private void TextBoxLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtHintLogin.Visibility = Visibility.Visible;
            if (TextBoxLogin.Text.Length > 0)
            {
                txtHintLogin.Visibility = Visibility.Hidden;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txtHintPass.Visibility = Visibility.Visible;
            if (PasswordBox.Password.Length > 0)
            {
                txtHintPass.Visibility = Visibility.Hidden;
            }
        }
    }
}
