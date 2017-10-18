using System.Windows.Input;
using JaCoCoReader.Models;
using JaCoCoReader.UI;

namespace JaCoCoReader.ViewModels
{
    public class MainViewModel : ViewModel<Report>
    {
        private FolderCollectionViewModel _folders;
        private IFolderNodeViewModel _selectedNode;

        public IFolderNodeViewModel SelectedNode
        {
            get { return _selectedNode; }
            set { SetProperty(ref _selectedNode, value); }
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
    }
}
