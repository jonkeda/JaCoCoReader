using JaCoCoReader.Core.ViewModels;
using JaCoCoReader.Core.ViewModels.CodeCoverage;
using JaCoCoReader.Core.ViewModels.Tests;

namespace JaCoCoReader.Vsix.Services
{
    public class CodeCoverageService
    {
        private static CodeCoverageService _current;
        public static CodeCoverageService Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new CodeCoverageService();
                }
                return _current;
            }
        }

        public CodeCoverageService()
        {
            Report = new ReportViewModel();
            Solution = new SolutionViewModel(Report);
        }

        public ReportViewModel Report { get; }
        public SolutionViewModel Solution { get; }
    }
}
