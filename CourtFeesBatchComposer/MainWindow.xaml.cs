using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CourtFeesBatchComposer.ExcelFunctions;
using CourtFeesBatchComposer.Model;
using CourtFeesBatchComposer.ViewModel;
using Microsoft.Win32;
using OfficeOpenXml;

namespace CourtFeesBatchComposer {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window {
        private CourtDocsViewModel courtDocs;
        private DataTable courtFeesDataTable;

        public MainWindow() {
            InitializeComponent();
            this.DataContext = courtDocs;
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog() {
                Filter = "txt files (*.txt)|*.txt|All files (*.*)| *.*",
                Multiselect = true,
            };
            if (openFileDialog.ShowDialog() == true) {
                var backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += new DoWorkEventHandler(EstablishViewModel);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ViewModelCreated);
                backgroundWorker.RunWorkerAsync(openFileDialog.FileNames);
            }
        }

        private void ViewModelCreated(object sender, RunWorkerCompletedEventArgs e) {
            this.FilePathBox.DataContext = courtDocs;
            this.CourtFeeListBox.ItemsSource = courtDocs.BatchedCourtFeesFiles;
        }

        private void EstablishViewModel(object sender, DoWorkEventArgs e) {
            courtDocs = new CourtDocsViewModel(e.Argument as string[]);
            courtDocs.BatchedCourtFeesFiles = courtDocs.BatchTogetherCourtFeeFiles();
            courtFeesDataTable = GetInDataTableFormat();
        }

        private void CourtFeeListBox_OnMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (CourtFeeListBox.SelectedItem != null) {
                var selection = CourtFeeListBox.SelectedItem as CourtFeesFile;
                Process.Start(selection.CourtFeeFileInfo.FullName);
            }
        }

        private void ExitApplication(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void GenerateExcel(object sender, RoutedEventArgs e) {
            var generateExcel = new Task(new Action(ExcelCreate));
            generateExcel.Start();
        }

        private void ExcelCreate() {
            
            using (
                var excelPackage =
                    new ExcelPackage(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Output - Fees.xlsx")))
                ) {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Process Server Court Fees");
                foreach (var file in courtDocs.BatchedCourtFeesFiles) {
                    worksheet.Cells["A1"].LoadFromDataTable(GetInDataTableFormat(), true);
                    
                }
                excelPackage.Save();
            }
        }

        //Move to Helper Class
        private DataTable GetInDataTableFormat() {
            DataTable tempTable = new DataTable();
            tempTable.Columns.Add("Defendant", typeof (string));
            tempTable.Columns.Add("IndexNumber", typeof(string));
            tempTable.Columns.Add("MatterNumber", typeof(string));
            tempTable.Columns.Add("CourtFee", typeof(decimal));
            tempTable.Columns.Add("InvoiceNumber", typeof(int));
            tempTable.Columns.Add("ProcessServerFileDate", typeof (string));
            tempTable.Columns.Add("DateCompleted", typeof (DateTime));

            foreach (var file in courtDocs.BatchedCourtFeesFiles) {
                foreach (var entry in file.CourtFees) {
                    tempTable.Rows.Add(entry.Defendant, entry.IndexNumber, entry.MatterNumber, entry.CourtFee,
                        entry.InvoiceNumber, file.CourtDate, entry.DateCompleted);
                }
            }
            courtFeesDataTable = tempTable;
            return tempTable;
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e) {
            try {
                var generateWinxfer = new Task(new Action(GenerateWinxferFile));
                generateWinxfer.Start();
            }
            catch (Exception ex) {
                MessageBox.Show("Error");
            }
        }

        private void GenerateWinxferFile() {
            DataView view = new DataView(courtFeesDataTable);
            DataTable onlyMatterNumbers = view.ToTable(false, "MatterNumber");
            using (var streamwriter = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CourtFeeMatterNumbers.txt"), false)) {
                foreach (DataRow row in onlyMatterNumbers.Rows) {
                    streamwriter.WriteLine(row["MatterNumber"].ToString());
                }
            }
        }
    }
}
