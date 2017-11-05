using System;
using System.Linq;
using System.Windows.Input;
using JaCoCoReader.Core.Models.CodeCoverage;
using JaCoCoReader.Core.UI;

namespace JaCoCoReader.Core.ViewModels.CodeCoverage
{
    public class ReportViewModel : FileViewModel<Report>
    {
        private FolderCollectionViewModel _folders;
        private IFolderNodeViewModel _selectedNode;
        private bool _showLinesHit = true;

        public IFolderNodeViewModel SelectedNode
        {
            get { return _selectedNode; }
            set { SetProperty(ref _selectedNode, value); }
        }

        public bool ShowLinesHit
        {
            get { return _showLinesHit; }
            set
            {
                SetProperty(ref _showLinesHit, value);
            }
        }

        public ICommand RefreshCommand
        {
            get { return new TargetCommand(DoRefreshCommand); }
        }

        private void DoRefreshCommand()
        {
            LoadModel(FileName);
        }

        protected override void OnModelChanged()
        {
            _folders = null;
           NotifyPropertyChanged(nameof(Folders));
        }


        public FolderCollectionViewModel Folders
        {
            get
            {
                if (_folders == null)
                {
                    FolderCollection folders = new FolderCollection();
                    if (Model?.Packages != null
                        && Model.Packages.Count > 0)
                    {
                        folders.Set(Model.Packages[0].Sourcefiles);
                        if (folders.Count == 1)
                        {
                            Folder folder = folders.FirstOrDefault();
                            while (folder != null
                                && folder.Folders.Count > 0
                                && folder.Folders.Count + folder.Sourcefiles.Count == 1)
                            {
                                folders = folder.Folders;
                                folder = folders.FirstOrDefault();
                            }
                        }
                        _folders = new FolderCollectionViewModel(folders);
                        
                    }
                }
                return _folders;
            }
        }

        public ICommand PreviousCommand
        {
            get { return new TargetCommand(DoPreviousCommand); }
        }

        private void DoPreviousCommand()
        {
        }

        public ICommand NextCommand
        {
            get { return new TargetCommand(DoNextCommand); }
        }

        private void DoNextCommand()
        {
            var node = Folders?.Find(i => i.MissedLines > 0);
            if (node != null)
            {
                node.IsSelected = true;
            }
        }

        public SourcefileViewModel GetSourceFileByPath(string textDocumentFilePath)
        {
            if (Model.Packages == null)
            {
                return null;
            }
            foreach (Package package in Model.Packages)
            {
                foreach (Sourcefile sourcefile in package.Sourcefiles)
                {
                    if (string.Equals(sourcefile.Name, textDocumentFilePath,
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        return new SourcefileViewModel(sourcefile);
                    }
                }
            }
            return null;
        }

        public void Merge(ReportCollection reports)
        {
            foreach (Report report in reports)
            {
                Model.Merge(report);
            }
            OnModelChanged();
        }
    }
}
