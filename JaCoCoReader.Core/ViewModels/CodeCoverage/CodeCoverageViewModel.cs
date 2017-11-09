using System;
using System.Linq;
using System.Windows.Input;
using JaCoCoReader.Core.Models.CodeCoverage;
using JaCoCoReader.Core.UI;
using System.Collections.Generic;
using JaCoCoReader.Core.Services;

namespace JaCoCoReader.Core.ViewModels.CodeCoverage
{
    public delegate void CodeCoverageChanged();

    public class CodeCoverageViewModel : FileViewModel<Report>
    {
        private static readonly List<Item<CoveredScripts>> _coveredScriptsItems = new List<Item<CoveredScripts>>
        {
            new Item<CoveredScripts>(CoveredScripts.SameNamedScripts, "Same named scripts"),
            new Item<CoveredScripts>(CoveredScripts.AllScripts, "All scripts"),
            //new Item<CoveredScripts>(CoveredScripts.SelectedScript, "Selected scripts"),
            new Item<CoveredScripts>(CoveredScripts.FromDescribeParameter, "From Describe parameter")
        };

        private FolderCollectionViewModel _folders;
        private IFolderNodeViewModel _selectedNode;
        private bool _showLinesHit = true;
        private Item<CoveredScripts> _selectedCoveredScripts;

        public List<Item<CoveredScripts>> CoveredScriptsItems
        {
            get
            {
                return _coveredScriptsItems;
            }
        }

        public Item<CoveredScripts> SelectedCoveredScriptsItem
        {
            get { return _selectedCoveredScripts ?? _coveredScriptsItems.FirstOrDefault(); }
            set { SetProperty(ref _selectedCoveredScripts, value); }
        }

        public CoveredScripts SelectedCoveredScripts
        {
            get
            {
                return SelectedCoveredScriptsItem?.Value ?? CoveredScripts.FromDescribeParameter;
            }
        }

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

        public ICommand ClearCommand
        {
            get { return new TargetCommand(DoClearCommand); }
        }

        private void DoClearCommand()
        {
            Model.Packages.Clear();
            OnModelChanged();
        }

        public ICommand OpenFileCommand
        {
            get { return new TargetCommand(DoOpenFileCommand); }
        }

        protected virtual void DoOpenFileCommand()
        {

        }

        protected override void OnModelChanged()
        {
            _folders = null;
            DoModelChanged();
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

        public event CodeCoverageChanged ModelChanged;

        private void DoModelChanged()
        {
            ModelChanged?.Invoke();
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
