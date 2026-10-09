using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for ChangePassPage.xaml
    /// </summary>
    public partial class ChangePassPage : Page
    {
        public ChangePassPage()
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

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentPasswordBox.Password) ||
                string.IsNullOrEmpty(NewPasswordBox.Password) ||
                string.IsNullOrEmpty(ConfirmPasswordBox.Password) ||
                string.IsNullOrEmpty(TbLogin.Text))
            {
                ShowMessage("Все поля обязательны к заполнению!");
                return;
            }

            string hashedPass = GetHash(CurrentPasswordBox.Password);
            var user = Entities.GetContext().Users
                .FirstOrDefault(u => u.Login == TbLogin.Text && u.Password == hashedPass);
            if (user == null)
            {
                ShowMessage("Текущий пароль или логин неверный!", "Ошибка");
                return;
            }

            if (NewPasswordBox.Password.Length < 6)
            {
                ShowMessage("Пароль слишком короткий, должно быть минимум 6 символов!", "Ошибка");
                return;
            }

            bool en = true;
            bool number = false;

            for (int i = 0; i < NewPasswordBox.Password.Length; i++)
            {
                if (NewPasswordBox.Password[i] >= '0' && NewPasswordBox.Password[i] <= '9')
                    number = true;
                else if (!((NewPasswordBox.Password[i] >= 'A' && NewPasswordBox.Password[i] <= 'Z') ||
                           (NewPasswordBox.Password[i] >= 'a' && NewPasswordBox.Password[i] <= 'z')))
                    en = false;
            }

            if (!en)
            {
                ShowMessage("Используйте только английскую раскладку!", "Ошибка");
                return;
            }
            if (!number)
            {
                ShowMessage("Добавьте хотя бы одну цифру!", "Ошибка");
                return;
            }
            if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
            {
                ShowMessage("Пароли не совпадают!", "Ошибка");
                return;
            }

            user.Password = GetHash(NewPasswordBox.Password);
            Entities.GetContext().SaveChanges();
            ShowMessage("Пароль успешно изменен!");
            NavigationService?.Navigate(new AuthPage());
        }

        private void lblLoginHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TbLogin.Focus();
        }

        private void lblCurrentPassHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CurrentPasswordBox.Focus();
        }

        private void lblNewPassHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NewPasswordBox.Focus();
        }

        private void lblConfirmPassHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ConfirmPasswordBox.Focus();
        }

        private void TbLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblLoginHint.Visibility = Visibility.Visible;
            if (TbLogin.Text.Length > 0)
            {
                lblLoginHint.Visibility = Visibility.Hidden;
            }
        }

        private void CurrentPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblCurrentPassHint.Visibility = Visibility.Visible;
            if (CurrentPasswordBox.Password.Length > 0)
            {
                lblCurrentPassHint.Visibility = Visibility.Hidden;
            }
        }

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblNewPassHint.Visibility = Visibility.Visible;
            if (NewPasswordBox.Password.Length > 0)
            {
                lblNewPassHint.Visibility = Visibility.Hidden;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblConfirmPassHint.Visibility = Visibility.Visible;
            if (ConfirmPasswordBox.Password.Length > 0)
            {
                lblConfirmPassHint.Visibility = Visibility.Hidden;
            }
        }

        private void TbLogin_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[a-zA-Z0-9@._-]+$");
        }

        private void TbLogin_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
    }
}
