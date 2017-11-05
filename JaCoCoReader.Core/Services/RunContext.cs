using JaCoCoReader.Core.Models.CodeCoverage;

namespace JaCoCoReader.Core.Services
{
    public class RunContext
    {
        public  string TestRunDirectory { get; set; }
        public string SolutionDirectory { get; set; }

        public ReportCollection Reports { get; } = new ReportCollection();

        public void AddCodeCoverageReport(Report report)
        {
            Reports.Add(report);
        }
    }
}