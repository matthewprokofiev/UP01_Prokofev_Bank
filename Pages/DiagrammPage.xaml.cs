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
using Word = Microsoft.Office.Interop.Word;

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

        private void BtnWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var allUsers = _context.Users.Where(u => u.Role == "User").ToList();
                var allBranches = _context.Branch.ToList();

                var application = new Word.Application();
                Word.Document document = application.Documents.Add();

                foreach (var user in allUsers)
                {
                    Word.Paragraph userParagraph = document.Paragraphs.Add();
                    Word.Range userRange = userParagraph.Range;
                    userRange.Text = user.FIO;
                    userParagraph.set_Style(Word.WdBuiltinStyle.wdStyleTitle);
                    userRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    userRange.InsertParagraphAfter();
                    document.Paragraphs.Add(); //Пустая строка

                    Word.Paragraph tableParagraph = document.Paragraphs.Add();
                    Word.Range tableRange = tableParagraph.Range;
                    Word.Table accountsTable = document.Tables.Add(tableRange, allBranches.Count() + 1, 2);
                    accountsTable.Borders.InsideLineStyle = accountsTable.Borders.OutsideLineStyle =
                        Word.WdLineStyle.wdLineStyleSingle;
                    accountsTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                    Word.Range cellRange;
                    cellRange = accountsTable.Cell(1, 1).Range;
                    cellRange.Text = "Отделение";
                    cellRange = accountsTable.Cell(1, 2).Range;
                    cellRange.Text = "Сумма остатков";
                    accountsTable.Rows[1].Range.Font.Name = "Times New Roman";
                    accountsTable.Rows[1].Range.Font.Size = 14;
                    accountsTable.Rows[1].Range.Bold = 1;
                    accountsTable.Rows[1].Range.ParagraphFormat.Alignment =
                        Word.WdParagraphAlignment.wdAlignParagraphCenter;

                    for (int i = 0; i < allBranches.Count(); i++)
                    {
                        var currentBranch = allBranches[i];
                        cellRange = accountsTable.Cell(i + 2, 1).Range;
                        cellRange.Text = currentBranch.Name;
                        cellRange.Font.Name = "Times New Roman";
                        cellRange.Font.Size = 12;

                        cellRange = accountsTable.Cell(i + 2, 2).Range;
                        cellRange.Text = user.Account.ToList()
                            .Where(a => a.Branch == currentBranch)
                            .Sum(a => a.Balance).ToString("N2") + " руб.";
                        cellRange.Font.Name = "Times New Roman";
                        cellRange.Font.Size = 12;
                    } //завершение цикла по строкам таблицы

                    document.Paragraphs.Add(); //пустая строка

                    Account maxAccount = user.Account.OrderByDescending(a => a.Balance).FirstOrDefault();
                    if (maxAccount != null)
                    {
                        Word.Paragraph maxAccountParagraph = document.Paragraphs.Add();
                        Word.Range maxAccountRange = maxAccountParagraph.Range;
                        maxAccountRange.Text = $"Счёт с наибольшим остатком: № {maxAccount.Number}, {maxAccount.Branch.Name}, " +
                            $"{maxAccount.Balance.ToString("N2")} руб., открыт {maxAccount.OpenDate.ToString("dd.MM.yyyy")}";
                        maxAccountParagraph.set_Style(Word.WdBuiltinStyle.wdStyleSubtitle);
                        maxAccountRange.Font.Color = Word.WdColor.wdColorDarkRed;
                        maxAccountRange.InsertParagraphAfter();
                    }
                    document.Paragraphs.Add(); //пустая строка

                    Account minAccount = user.Account.OrderBy(a => a.Balance).FirstOrDefault();
                    if (minAccount != null)
                    {
                        Word.Paragraph minAccountParagraph = document.Paragraphs.Add();
                        Word.Range minAccountRange = minAccountParagraph.Range;
                        minAccountRange.Text = $"Счёт с наименьшим остатком: № {minAccount.Number}, {minAccount.Branch.Name}, " +
                            $"{minAccount.Balance.ToString("N2")} руб., открыт {minAccount.OpenDate.ToString("dd.MM.yyyy")}";
                        minAccountParagraph.set_Style(Word.WdBuiltinStyle.wdStyleSubtitle);
                        minAccountRange.Font.Color = Word.WdColor.wdColorDarkGreen;
                        minAccountRange.InsertParagraphAfter();
                    }

                    if (user != allUsers.LastOrDefault())
                        document.Words.Last.InsertBreak(Word.WdBreakType.wdPageBreak);
                } //завершение цикла по пользователям

                foreach (Word.Section section in document.Sections)
                {
                    // Получаем нижний колонтитул
                    Word.HeaderFooter footer = section.Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary];
                    // Добавляем номер страницы в центр нижнего колонтитула
                    footer.PageNumbers.Add(Word.WdPageNumberAlignment.wdAlignPageNumberCenter);
                }

                foreach (Word.Section section in document.Sections)
                {
                    Word.Range headerRange = section.Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                    headerRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    headerRange.Font.ColorIndex = Word.WdColorIndex.wdBlack;
                    headerRange.Font.Size = 10;
                    headerRange.Text = DateTime.Now.ToString("dd.MM.yyyy");
                }

                application.Visible = true;
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                document.SaveAs2(System.IO.Path.Combine(folder, "Accounts.docx"));
                document.SaveAs2(System.IO.Path.Combine(folder, "Accounts.pdf"), Word.WdSaveFormat.wdFormatPDF);
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, "Ошибка");
            }
        }
    }
}
