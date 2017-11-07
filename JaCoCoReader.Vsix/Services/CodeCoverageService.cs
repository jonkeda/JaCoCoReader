using JaCoCoReader.Core.ViewModels.CodeCoverage;
using JaCoCoReader.Core.ViewModels.Tests;
using JaCoCoReader.Vsix.ViewModels;

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
            Report = new VsReportViewModel();
            Solution = new VsSolutionViewModel(Report);
        }

        public ReportViewModel Report { get; }
        public SolutionViewModel Solution { get; }
    }
}
