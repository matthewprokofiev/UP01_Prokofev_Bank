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

namespace UP01_Prokofev_Bank.Pages
{
    /// <summary>
    /// Interaction logic for AccountTabPage.xaml
    /// </summary>
    public partial class AccountTabPage : Page
    {
        public AccountTabPage()
        {
            InitializeComponent();
            DataGridAccount.ItemsSource = Entities.GetContext().Account.ToList();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void ShowMessage(string text, string caption = "Сообщение")
        {
            MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DataGridAccount.ItemsSource = Entities.GetContext().Account.ToList();
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddAccountPage(null));
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var accountForRemoving = DataGridAccount.SelectedItems.Cast<Account>().ToList();
            if (accountForRemoving.Count == 0)
            {
                ShowMessage("Выберите записи для удаления");
                return;
            }
            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {accountForRemoving.Count()} элементов?",
                "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Entities.GetContext().Account.RemoveRange(accountForRemoving);
                    Entities.GetContext().SaveChanges();
                    ShowMessage("Данные успешно удалены!");
                    DataGridAccount.ItemsSource = Entities.GetContext().Account.ToList();
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message.ToString(), "Ошибка");
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Pages.AddAccountPage((sender as Button).DataContext as Account));
        }
    }
}
