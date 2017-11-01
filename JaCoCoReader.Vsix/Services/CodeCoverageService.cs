using JaCoCoReader.Core.ViewModels;

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

        public ReportViewModel Report { get; } = new ReportViewModel();
    }
}
