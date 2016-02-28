using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CourtFeesBatchComposer.Model;
using CourtFeesBatchComposer.ViewModel;
using Microsoft.Win32;

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
    }
}
