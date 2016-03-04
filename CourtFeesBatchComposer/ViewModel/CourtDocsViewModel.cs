using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CourtFeesBatchComposer.Annotations;
using CourtFeesBatchComposer.Model;

namespace CourtFeesBatchComposer.ViewModel {
    public class CourtDocsViewModel : INotifyPropertyChanged {
        private int _numberOfFiles;
        private string[] _fileDirectoryPath;
        private string _fileBeingProcessed;
        private string _mainDirectory;
     
 
    
        public CourtDocsViewModel(string[] batchFilesDirectoryPaths) {
            FileDirectoryPath = batchFilesDirectoryPaths;
            MainDirectory = batchFilesDirectoryPaths[0];

        }

        public int NumberOfFiles {
            get { return _numberOfFiles;}
            set {
                _numberOfFiles = value;
                OnPropertyChanged(nameof(NumberOfFiles));
            }
        }

        public string[] FileDirectoryPath {
            get { return _fileDirectoryPath; }
            set {
                _fileDirectoryPath = value;
                OnPropertyChanged(nameof(FileDirectoryPath));
            }
        }

        public string FileBeingProcessed {
            get { return _fileBeingProcessed; }
            set {
                _fileBeingProcessed = value;
                OnPropertyChanged(nameof(FileBeingProcessed));
            }
        }

        public string MainDirectory {
            get { return _mainDirectory;}
            set {
                _mainDirectory = value;
                OnPropertyChanged(nameof(MainDirectory));
            }
            
        }

        public ObservableCollection<CourtFeesFile> BatchTogetherCourtFeeFiles() {
            ObservableCollection<CourtFeesFile> tempBatchCourtFeesFiles = new ObservableCollection<CourtFeesFile>();
            foreach (var file in FileDirectoryPath) {
                MainDirectory = file;
                tempBatchCourtFeesFiles.Add(new CourtFeesFile(file.ToString()));
            }

            return tempBatchCourtFeesFiles;
        } 

        public ObservableCollection<CourtFeesFile> BatchedCourtFeesFiles { get; set; } 


        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
