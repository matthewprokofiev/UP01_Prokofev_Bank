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
    /// Interaction logic for AddUserPage.xaml
    /// </summary>
    public partial class AddUserPage : Page
    {
        private Users _currentUser = new Users();
        private string _oldPassword;

        public AddUserPage(Users selectedUser)
        {
            InitializeComponent();
            if (selectedUser != null)
                _currentUser = selectedUser;
            _oldPassword = _currentUser.Password;
            DataContext = _currentUser;
            Title = selectedUser == null ? "Добавление пользователя" : "Редактирование пользователя";
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
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentUser.Login))
                errors.AppendLine("Укажите логин!");
            else if (!Regex.IsMatch(_currentUser.Login, "^[a-zA-Z0-9@._-]+$"))
                errors.AppendLine("Логин может содержать только английские буквы, цифры и символы @ . _ -");
            if (string.IsNullOrWhiteSpace(_currentUser.Password))
                errors.AppendLine("Укажите пароль!");
            if (cmbRole.SelectedItem == null)
                errors.AppendLine("Выберите роль!");
            if (string.IsNullOrWhiteSpace(_currentUser.FIO))
                errors.AppendLine("Укажите ФИО!");
            else if (!Regex.IsMatch(_currentUser.FIO, "^[а-яА-ЯёЁ -]+$"))
                errors.AppendLine("ФИО может содержать только русские буквы, пробел и дефис");

            if (errors.Length > 0)
            {
                ShowMessage(errors.ToString(), "Ошибка");
                return;
            }

            if (Entities.GetContext().Users.Any(u => u.Login == _currentUser.Login && u.ID != _currentUser.ID))
            {
                ShowMessage("Пользователь с таким логином уже существует!", "Ошибка");
                return;
            }

            if (_currentUser.Password != _oldPassword)
            {
                string password = _currentUser.Password;
                if (password.Length < 6)
                {
                    ShowMessage("Пароль слишком короткий, должно быть минимум 6 символов!", "Ошибка");
                    return;
                }

                bool en = true;
                bool number = false;

                for (int i = 0; i < password.Length; i++)
                {
                    if (password[i] >= '0' && password[i] <= '9')
                        number = true;
                    else if (!((password[i] >= 'A' && password[i] <= 'Z') ||
                               (password[i] >= 'a' && password[i] <= 'z')))
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

                _currentUser.Password = GetHash(_currentUser.Password);
                _oldPassword = _currentUser.Password;
            }
            if (string.IsNullOrWhiteSpace(_currentUser.Photo))
                _currentUser.Photo = null;

            if (_currentUser.ID == 0)
                Entities.GetContext().Users.Add(_currentUser);

            try
            {
                Entities.GetContext().SaveChanges();
                ShowMessage("Данные успешно сохранены!");
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message.ToString(), "Ошибка");
            }
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            TBLogin.Text = "";
            TBPass.Text = "";
            cmbRole.SelectedIndex = -1;
            TBFio.Text = "";
            TBPhoto.Text = "";
        }

        private void TBLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblLoginHint.Visibility = Visibility.Visible;
            if (TBLogin.Text.Length > 0)
            {
                lblLoginHint.Visibility = Visibility.Hidden;
            }
        }

        private void TBPass_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblPassHint.Visibility = Visibility.Visible;
            if (TBPass.Text.Length > 0)
            {
                lblPassHint.Visibility = Visibility.Hidden;
            }
        }

        private void TBFio_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblFioHint.Visibility = Visibility.Visible;
            if (TBFio.Text.Length > 0)
            {
                lblFioHint.Visibility = Visibility.Hidden;
            }
        }

        private void TBPhoto_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblPhotoHint.Visibility = Visibility.Visible;
            if (TBPhoto.Text.Length > 0)
            {
                lblPhotoHint.Visibility = Visibility.Hidden;
            }
        }

        private void lblLoginHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TBLogin.Focus();
        }

        private void lblPassHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TBPass.Focus();
        }

        private void lblFioHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TBFio.Focus();
        }

        private void lblPhotoHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TBPhoto.Focus();
        }

        private void TBLogin_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[a-zA-Z0-9@._-]+$");
        }

        private void TBLogin_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void TBFio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[а-яА-ЯёЁ -]+$");
        }
    }
}
