using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Media;
using JaCoCoReader.Core.UI;
using JaCoCoReader.Core.UI.Icons;

namespace JaCoCoReader.Core.ViewModels.CodeCoverage
{
    public class FolderViewModel : NodeViewModel<Folder>, IFolderNodeViewModel
    {
        public FolderViewModel(Folder folder) : base(folder)
        {
            Folders = new Lazy<FolderCollectionViewModel>(() => new FolderCollectionViewModel(Model.Folders));
            Sourcefiles = new Lazy<SourcefileCollectionViewModel>(() => new SourcefileCollectionViewModel(Model.Sourcefiles));
        }
        public override string Description
        {
            get { return Model.Name; }
        }

        public override FontAwesomeIcon Icon
        {
            get { return FontAwesomeIcon.FolderOpen; }
        }

        //public Lazy<ObservableViewModelCollection<FolderViewModel, Folder> Folders { get; }

        public Lazy<FolderCollectionViewModel> Folders { get; }
        public Lazy<SourcefileCollectionViewModel> Sourcefiles { get; }

        private Collection<IFolderNodeViewModel> _items;

        public IEnumerable<IFolderNodeViewModel> Items
        {
            get
            {
                if (Model == null)
                {
                    return null;
                }
                if (_items == null)
                {
                    _items = new Collection<IFolderNodeViewModel>();
                    foreach (FolderViewModel folder in Folders.Value.Nodes)
                    {
                        _items.Add(folder);
                        //yield return folder;
                    }
                    foreach (SourcefileViewModel sourceFiles in Sourcefiles.Value.Nodes)
                    {
                        _items.Add(sourceFiles);
                        //yield return sourceFiles;
                    }
                }
                return _items;
               
            }
        }

        public int MissedLines
        {
            get { return Model.MissedLines; }
        }

        public int CoveredLines
        {
            get { return Model.CoveredLines; }
        }

        public double CoveredLinesPercentage
        {
            get { return CoveredLines / (double)TotalLines; }
        }

        public double MissedLinesPercentage
        {
            get { return MissedLines / (double)TotalLines; }
        }

        public int TotalLines
        {
            get
            {
                return CoveredLines + MissedLines;
            }
        }

        public Brush Background
        {
            get
            {
                if (MissedLines > 0)
                {
                    return Brushes.DarkRed;
                }
                if (CoveredLines > 0)
                {
                    return Brushes.DarkGreen;
                }
                return Colors.DefaultBackground;
            }
        }

        public IFolderNodeViewModel Find(Func<IFolderNodeViewModel, bool> func)
        {
            foreach (FolderViewModel folder in Folders.Value.Nodes)
            {
                var foundFolder = folder.Find(func);
                if (foundFolder != null)
                {
                    return foundFolder;
                }
            }
            foreach (SourcefileViewModel sourcefile in Sourcefiles.Value.Nodes)
            {
                if (func(sourcefile))
                {
                    return sourcefile;
                }
            }
            return null;
        }
    }
}