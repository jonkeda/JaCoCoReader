using System;
using System.Collections.Generic;
using System.Windows.Media;
using JaCoCoReader.Core.UI;

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

        public Lazy<FolderCollectionViewModel> Folders { get; }
        public Lazy<SourcefileCollectionViewModel> Sourcefiles { get; }

        public IEnumerable<IFolderNodeViewModel> Items
        {
            get
            {
                if (Model == null)
                {
                    yield break;
                }
                foreach (FolderViewModel folder in Folders.Value.Nodes)
                {
                    yield return folder;
                }
                foreach (SourcefileViewModel sourceFiles in Sourcefiles.Value.Nodes)
                {
                    yield return sourceFiles;
                }

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