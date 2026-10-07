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
    /// Interaction logic for AddBranchPage.xaml
    /// </summary>
    public partial class AddBranchPage : Page
    {
        private Branch _currentBranch = new Branch();

        public AddBranchPage(Branch selectedBranch)
        {
            InitializeComponent();
            if (selectedBranch != null)
                _currentBranch = selectedBranch;
            DataContext = _currentBranch;
            Title = selectedBranch == null ? "Добавление отделения" : "Редактирование отделения";
        }

        private void ShowMessage(string text, string caption = "Сообщение")
        {
            MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonSaveBranch_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentBranch.Name))
                errors.AppendLine("Укажите название отделения!");
            if (string.IsNullOrWhiteSpace(_currentBranch.Address))
                errors.AppendLine("Укажите адрес!");
            if (_currentBranch.Phone == null || !Regex.IsMatch(_currentBranch.Phone, "^[0-9]{11}$"))
                errors.AppendLine("Телефон должен состоять ровно из 11 цифр!");
            if (errors.Length > 0)
            {
                ShowMessage(errors.ToString(), "Ошибка");
                return;
            }

            if (_currentBranch.ID == 0)
                Entities.GetContext().Branch.Add(_currentBranch);

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
            TBBranchName.Text = "";
            TBAddress.Text = "";
            TBPhone.Text = "";
        }

        private void TBBranchName_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblBranchNameHint.Visibility = Visibility.Visible;
            if (TBBranchName.Text.Length > 0)
            {
                lblBranchNameHint.Visibility = Visibility.Hidden;
            }
        }

        private void TBAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblAddressHint.Visibility = Visibility.Visible;
            if (TBAddress.Text.Length > 0)
            {
                lblAddressHint.Visibility = Visibility.Hidden;
            }
        }

        private void TBPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblPhoneHint.Visibility = Visibility.Visible;
            if (TBPhone.Text.Length > 0)
            {
                lblPhoneHint.Visibility = Visibility.Hidden;
            }
        }

        private void lblBranchNameHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TBBranchName.Focus();
        }

        private void lblAddressHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TBAddress.Focus();
        }

        private void lblPhoneHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TBPhone.Focus();
        }
    }
}
