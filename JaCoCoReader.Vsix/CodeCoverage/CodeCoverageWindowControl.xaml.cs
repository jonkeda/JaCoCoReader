using JaCoCoReader.Vsix.Services;

namespace JaCoCoReader.Vsix.CodeCoverage
{
    public partial class CodeCoverageWindowControl
    {
        public CodeCoverageWindowControl()
        {
            InitializeComponent();

            DataContext = PowershellService.Current.CodeCoverage;
        }
    }
}