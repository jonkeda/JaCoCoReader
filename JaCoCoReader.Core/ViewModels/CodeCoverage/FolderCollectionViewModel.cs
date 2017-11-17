using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using JaCoCoReader.Core.UI;

namespace JaCoCoReader.Core.ViewModels.CodeCoverage
{
    public class FolderCollectionViewModel : NodeCollectionViewModel<FolderCollection, FolderViewModel>
    {
        public FolderCollectionViewModel(FolderCollection model)
            : base(model)
        {
        }

        public override string Description { get; } = "Folders";
        private Collection<FolderViewModel> _nodes;
        public override IEnumerable<FolderViewModel> Nodes
        {
            get
            {
                if (Model == null)
                {
                    return null;
                }
                if (_nodes == null)
                {
                    _nodes = new Collection<FolderViewModel>();
                    foreach (Folder node in Model)
                    {
                        _nodes.Add(new FolderViewModel(node));
                    }
                }
                return _nodes;
            }
        }


        public IFolderNodeViewModel Find(Func<IFolderNodeViewModel, bool> func)
        {
            foreach (FolderViewModel folder in Nodes)
            {
                var foundFolder = folder.Find(func);
                if (foundFolder != null)
                {
                    return foundFolder;
                }
            }
            return null;
        }
    }
}