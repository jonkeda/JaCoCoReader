using System.Collections.Generic;
using JaCoCoReader.Core.Models.CodeCoverage;
using JaCoCoReader.Core.UI;

namespace JaCoCoReader.Core.ViewModels.CodeCoverage
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