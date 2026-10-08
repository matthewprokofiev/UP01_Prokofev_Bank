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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UP01_Prokofev_Bank.Pages
{
    /// <summary>
    /// Interaction logic for AddAccountPage.xaml
    /// </summary>
    public partial class AddAccountPage : Page
    {
        private Account _currentAccount = new Account();

        public AddAccountPage(Account selectedAccount)
        {
            InitializeComponent();
            CBUser.ItemsSource = Entities.GetContext().Users.Where(u => u.Role == "User").ToList();
            CBBranch.ItemsSource = Entities.GetContext().Branch.ToList();
            if (selectedAccount != null)
            {
                _currentAccount = selectedAccount;
                TBBalance.Text = _currentAccount.Balance.ToString("0.00");
            }
            else
            {
                _currentAccount.OpenDate = DateTime.Today;
            }
            DataContext = _currentAccount;
            Title = selectedAccount == null ? "Добавление счёта" : "Редактирование счёта";
        }

        private void ShowMessage(string text, string caption = "Сообщение")
        {
            MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (_currentAccount.Number == null || !Regex.IsMatch(_currentAccount.Number, "^[0-9]{20}$"))
                errors.AppendLine("Номер счёта должен состоять ровно из 20 цифр!");

            decimal balance;
            if (!decimal.TryParse(TBBalance.Text, out balance))
                errors.AppendLine("Укажите остаток числом!");
            else if (balance < 0 || balance > 100000000)
                errors.AppendLine("Остаток должен быть от 0 до 100 000 000!");

            if (DPOpenDate.SelectedDate == null)
                errors.AppendLine("Укажите дату открытия!");
            else if (DPOpenDate.SelectedDate.Value > DateTime.Today)
                errors.AppendLine("Дата открытия не может быть позже сегодняшней!");
            else if (DPOpenDate.SelectedDate.Value < new DateTime(2000, 1, 1))
                errors.AppendLine("Дата открытия не может быть раньше 01.01.2000!");

            if (CBUser.SelectedItem == null)
                errors.AppendLine("Выберите клиента!");
            if (CBBranch.SelectedItem == null)
                errors.AppendLine("Выберите отделение!");

            if (errors.Length > 0)
            {
                ShowMessage(errors.ToString(), "Ошибка");
                return;
            }

            if (Entities.GetContext().Account.Any(a => a.Number == _currentAccount.Number && a.ID != _currentAccount.ID))
            {
                ShowMessage("Счёт с таким номером уже существует!", "Ошибка");
                return;
            }

            _currentAccount.Balance = balance;
            _currentAccount.OpenDate = DPOpenDate.SelectedDate.Value;

            if (_currentAccount.ID == 0)
                Entities.GetContext().Account.Add(_currentAccount);

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
            TBNumber.Text = "";
            TBBalance.Text = "";
            CBUser.SelectedIndex = -1;
            CBBranch.SelectedIndex = -1;
            DPOpenDate.SelectedDate = null;
        }

        private void TBNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblNumberHint.Visibility = Visibility.Visible;
            if (TBNumber.Text.Length > 0)
            {
                lblNumberHint.Visibility = Visibility.Hidden;
            }
        }

        private void TBBalance_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblBalanceHint.Visibility = Visibility.Visible;
            if (TBBalance.Text.Length > 0)
            {
                lblBalanceHint.Visibility = Visibility.Hidden;
            }
        }

        private void lblNumberHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TBNumber.Focus();
        }

        private void lblBalanceHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TBBalance.Focus();
        }

        private void TBNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void TBNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void TBBalance_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(Regex.IsMatch(e.Text, "^[0-9]+$") || (e.Text == "," && !TBBalance.Text.Contains(",")));
        }

        private void TBBalance_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
    }
}
