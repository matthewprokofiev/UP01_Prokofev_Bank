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

namespace UP01_Prokofev_Bank
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ChangeTheme(Properties.Settings.Default.Theme);
        }

        private void ChangeTheme(string fileName)
        {
            // определяем путь к файлу ресурсов
            var uri = new Uri(fileName, UriKind.Relative);
            // загружаем словарь ресурсов
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            // очищаем коллекцию ресурсов приложения
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Clear();
            // добавляем загруженный словарь ресурсов
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
            Properties.Settings.Default.Theme = fileName;
            Properties.Settings.Default.Save();
        }

        private void btnTheme_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.Theme == "Dictionary.xaml")
                ChangeTheme("DarkDictionary.xaml");
            else
                ChangeTheme("Dictionary.xaml");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.IsEnabled = true;
            timer.Tick += (o, t) => { DateTimeNow.Text = DateTime.Now.ToString(); };
            timer.Start();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
                MainFrame.GoBack();
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            Page page = MainFrame.Content as Page;
            if (page != null)
            {
                txtTitle.Text = page.Title;
                Title = "Прогресс-Банк: " + page.Title;
            }
            btnBack.Visibility = MainFrame.CanGoBack ? Visibility.Visible : Visibility.Hidden;
        }

        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите выйти?", "Выход",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                e.Cancel = true;
            else
                e.Cancel = false;
        }
    }
}
