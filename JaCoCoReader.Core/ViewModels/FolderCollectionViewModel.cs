using System;
using System.Collections.Generic;
using JaCoCoReader.Core.UI;

namespace JaCoCoReader.Core.ViewModels
{
    public class FolderCollectionViewModel : NodeCollectionViewModel<FolderCollection, FolderViewModel>
    {
        public FolderCollectionViewModel(FolderCollection model)
            : base(model)
        {
        }

        public override string Description { get; } = "Folders";
        public override IEnumerable<FolderViewModel> Nodes
        {
            get
            {
                if (Model == null)
                {
                    yield break;
                }

                foreach (Folder node in Model)
                {
                    yield return new FolderViewModel(node);
                }
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