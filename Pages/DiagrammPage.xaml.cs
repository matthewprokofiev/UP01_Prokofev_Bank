using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UP01_Prokofev_Bank.Pages
{
    /// <summary>
    /// Interaction logic for DiagrammPage.xaml
    /// </summary>
    public partial class DiagrammPage : Page
    {
        private Entities _context = new Entities();

        public DiagrammPage()
        {
            InitializeComponent();
            ChartAccounts.ChartAreas.Add(new ChartArea("Main"));
            var currentSeries = new Series("Остатки")
            {
                IsValueShownAsLabel = true
            };
            ChartAccounts.Series.Add(currentSeries);
            CmbUser.ItemsSource = _context.Users.Where(u => u.Role == "User").ToList();
            CmbDiagram.ItemsSource = Enum.GetValues(typeof(SeriesChartType));
        }

        private void ShowMessage(string text, string caption = "Сообщение")
        {
            MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            if (CmbUser.SelectedItem is Users currentUser &&
                CmbDiagram.SelectedItem is SeriesChartType currentType)
            {
                Series currentSeries = ChartAccounts.Series.FirstOrDefault();
                currentSeries.ChartType = currentType;
                currentSeries.Points.Clear();

                var branchList = _context.Branch.ToList();
                foreach (var branch in branchList)
                {
                    currentSeries.Points.AddXY(branch.Name,
                        _context.Account.ToList().Where(a => a.Users == currentUser && a.Branch == branch)
                            .Sum(a => a.Balance));
                }
            }
        }
    }
}
