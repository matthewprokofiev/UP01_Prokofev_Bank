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
using Excel = Microsoft.Office.Interop.Excel;

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
            var mainArea = new ChartArea("Main");
            mainArea.AxisY.LabelStyle.Format = "N0";
            mainArea.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.WordWrap;
            ChartAccounts.ChartAreas.Add(mainArea);
            var currentSeries = new Series("Остатки")
            {
                IsValueShownAsLabel = true,
                LabelFormat = "N2",
                LabelBackColor = System.Drawing.Color.White
            };
            currentSeries["PieLabelStyle"] = "Outside";
            currentSeries["PieLineColor"] = "Black";
            currentSeries["MinimumRelativePieSize"] = "60";
            currentSeries["BarLabelStyle"] = "Outside";
            currentSeries["LabelValueType"] = "High";
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
                    decimal sum = _context.Account.ToList().Where(a => a.Users == currentUser && a.Branch == branch)
                        .Sum(a => a.Balance);
                    currentSeries.Points.AddXY(branch.Name, sum);
                    if (sum == 0)
                        currentSeries.Points.Last()["PieLabelStyle"] = "Disabled";
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

        private void BtnExcel_Click(object sender, RoutedEventArgs e)
        {
            var allUsers = _context.Users.Where(u => u.Role == "User").ToList().OrderBy(u => u.FIO).ToList();
            if (allUsers.Count == 0)
            {
                ShowMessage("Нет клиентов для выгрузки");
                return;
            }

            try
            {
                var application = new Excel.Application();
                int oldSheetsCount = application.SheetsInNewWorkbook;
                application.SheetsInNewWorkbook = allUsers.Count();
                Excel.Workbook workbook = application.Workbooks.Add(Type.Missing);
                application.SheetsInNewWorkbook = oldSheetsCount;

                for (int i = 0; i < allUsers.Count(); i++)
                {
                    int startRowIndex = 1;
                    Excel.Worksheet worksheet = application.Worksheets.Item[i + 1];
                    worksheet.Name = allUsers[i].FIO.Length > 31 ? allUsers[i].FIO.Substring(0, 31) : allUsers[i].FIO;

                    worksheet.Cells[1][startRowIndex] = "Дата открытия";
                    worksheet.Cells[2][startRowIndex] = "Номер счёта";
                    worksheet.Cells[3][startRowIndex] = "Остаток, руб.";
                    startRowIndex++;

                    var userBranches = allUsers[i].Account.OrderBy(a => a.OpenDate)
                        .GroupBy(a => a.Branch).OrderBy(g => g.Key.Name);

                    foreach (var groupBranch in userBranches)
                    {
                        Excel.Range headerRange = worksheet.Range[worksheet.Cells[1][startRowIndex], worksheet.Cells[3][startRowIndex]];
                        headerRange.Merge();
                        headerRange.Value = groupBranch.Key.Name;
                        headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        headerRange.Font.Italic = true;
                        startRowIndex++;

                        foreach (var account in groupBranch)
                        {
                            worksheet.Cells[1][startRowIndex] = account.OpenDate.ToString("dd.MM.yyyy");
                            worksheet.Cells[2][startRowIndex] = "'" + account.Number;
                            worksheet.Cells[3][startRowIndex] = (double)account.Balance;
                            worksheet.Cells[3][startRowIndex].NumberFormat = "#,##0.00";
                            startRowIndex++;
                        }

                        Excel.Range sumRange = worksheet.Range[worksheet.Cells[1][startRowIndex], worksheet.Cells[2][startRowIndex]];
                        sumRange.Merge();
                        sumRange.Value = "ИТОГО:";
                        sumRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                        worksheet.Cells[3][startRowIndex].Formula =
                            $"=SUM(C{startRowIndex - groupBranch.Count()}:C{startRowIndex - 1})";
                        worksheet.Cells[3][startRowIndex].NumberFormat = "#,##0.00";
                        sumRange.Font.Bold = worksheet.Cells[3][startRowIndex].Font.Bold = true;
                        startRowIndex++;
                    }

                    Excel.Range rangeBorders = worksheet.Range[worksheet.Cells[1][1], worksheet.Cells[3][startRowIndex - 1]];
                    rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle =
                    rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle =
                    rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle =
                    rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle =
                    rangeBorders.Borders[Excel.XlBordersIndex.xlInsideHorizontal].LineStyle =
                    rangeBorders.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle =
                        Excel.XlLineStyle.xlContinuous;

                    worksheet.Columns.AutoFit();
                }

                application.Visible = true;
                application.DisplayAlerts = false;
                workbook.SaveAs(System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Accounts.xlsx"));
                application.DisplayAlerts = true;
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, "Ошибка");
            }
        }
    }
}
