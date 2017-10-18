using System.Collections.Generic;
using JaCoCoReader.Models;
using JaCoCoReader.UI;

namespace JaCoCoReader.ViewModels
{
    public class SourcefileCollectionViewModel : NodeCollectionViewModel<SourcefileCollection, SourcefileViewModel>
    {
        public SourcefileCollectionViewModel(SourcefileCollection model)
            : base(model)
        {
        }

        public override string Description { get; } = "Sourcefiles";
        public override IEnumerable<SourcefileViewModel> Nodes
        {
            get
            {
                if (Model == null)
                {
                    yield break;
                }

                foreach (Sourcefile node in Model)
                {
                    yield return new SourcefileViewModel(node);
                }
            }
        }
    }
}