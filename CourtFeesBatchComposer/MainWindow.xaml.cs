﻿using System;
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
                    new ExcelPackage(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fracktest.xlsx")))
                ) {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Process Server Court Fees");
                foreach (var file in courtDocs.BatchedCourtFeesFiles) {
                    worksheet.Cells["A1"].LoadFromCollection(file.CourtFees, true);
                    excelPackage.Save();
                }
            }
        }


        private DataTable GetInDataTableFormat() {
            DataTable tempTable = new DataTable();
            tempTable.Columns.Add("Defendant", typeof (string));
            tempTable.Columns.Add("IndexNumber", typeof(int));
            tempTable.Columns.Add("MatterNumber", typeof(string));
            tempTable.Columns.Add("CourtFee", typeof(decimal));
            tempTable.Columns.Add("InvoiceNumber", typeof(int));
            tempTable.Columns.Add("ProcessServerFileDate", typeof (string));

            foreach (var file in courtDocs.BatchedCourtFeesFiles) {
                foreach (var entry in file.CourtFees) {
                    tempTable.Rows.Add(entry.Defendant, entry.IndexNumber, entry.MatterNumber, entry.CourtFee,
                        entry.InvoiceNumber, file.CourtDate);
                }
            }
            return null;
        }

    }
}
