using JaCoCoReader.Core.ViewModels.CodeCoverage;
using JaCoCoReader.Vsix.Extensions;

namespace JaCoCoReader.Vsix.ViewModels
{
    public class VsCodeCoverageViewModel : CodeCoverageViewModel
    {
        protected override void DoOpenFileCommand()
        {
            if (SelectedNode is SourcefileViewModel file)
            {
                VsExtensions.OpenProjectItem(file.FileName);
            }
        }
    }
}