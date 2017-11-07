using System;
using System.Collections.Generic;
using JaCoCoReader.Core.Models.CodeCoverage;

namespace JaCoCoReader.Core.Services
{
    public class RunContext
    {
        private readonly Action<string> _running;
        public List<string> ScriptFileNames { get; }
        public CoveredScripts CoveredScripts { get; }

        public RunContext(Action<string> running, CoveredScripts coveredScripts, List<string> scriptFileNames)
        {
            _running = running;
            ScriptFileNames = scriptFileNames;
            CoveredScripts = coveredScripts;
        }

        public void UpdateRunningTest(string name)
        {
            _running(name);
        }

        public  string TestRunDirectory { get; set; }
        public string SolutionDirectory { get; set; }

        public ReportCollection Reports { get; } = new ReportCollection();

        public void AddCodeCoverageReport(Report report)
        {
            Reports.Add(report);
        }
    }
}