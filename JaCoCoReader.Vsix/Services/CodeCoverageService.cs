using JaCoCoReader.Core.ViewModels.CodeCoverage;
using JaCoCoReader.Core.ViewModels.Tests;
using JaCoCoReader.Vsix.ViewModels;

namespace JaCoCoReader.Vsix.Services
{
    public class PowershellService
    {
        private static PowershellService _current;
        public static PowershellService Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new PowershellService();
                }
                return _current;
            }
        }

        public PowershellService()
        {
            CodeCoverage = new VsCodeCoverageViewModel();
            Tests = new VsTestsViewModel(CodeCoverage);
        }

        public CodeCoverageViewModel CodeCoverage { get; }
        public TestsViewModel Tests { get; }
    }
}
