using PowerShellTools.TestAdapter;

namespace JaCoCoReader.Core.Services
{
    public class PowerShellTestResult
    {
        public PowerShellTestResult(TestOutcome outcome)
        {
            Outcome = outcome;
        }

        public PowerShellTestResult(TestOutcome outcome, string errorMessage, string errorStacktrace)
        {
            Outcome = outcome;
            ErrorMessage = errorMessage;
            ErrorStacktrace = errorStacktrace;
        }

        public TestOutcome Outcome { get; }
        public string ErrorMessage { get; }
        public string ErrorStacktrace { get; }
    }
}
