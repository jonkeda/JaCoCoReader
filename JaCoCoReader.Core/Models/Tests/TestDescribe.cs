using System;

using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using JaCoCoReader.Core.Services;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestDescribe : TestFileModel<TestFile>
    {
        public TestDescribe()
        {
            Contexts = new TestContextCollection(this);
        }

        public TestContextCollection Contexts { get; }

        public Ast Ast { get; set; }

        public override string Type
        {
            get { return "Describe"; }
        }

        public override IEnumerable<TestModel> Items
        {
            get { return Contexts; }
        }

        public void ProcessTestResults(Array results)
        {
            TestOutcome outcome = TestOutcome.None;

            foreach (PSObject result in results)
            {
                string describe = result.Properties["Describe"].Value as string;
                if (!HandleParseError(result, describe))
                {
                    break;
                }

                string context = result.Properties["Context"].Value as string;
                string name = result.Properties["Name"].Value as string;

                if (string.IsNullOrEmpty(context))
                {
                    context = "No Context";
                }

                TestContext testContext = Contexts.FirstOrDefault(c => c.Name == context);

                // Skip test cases we aren't trying to run
                TestIt testIt = testContext?.Its.FirstOrDefault(m => m.Name == name);
                if (testIt == null)
                {
                    continue;
                }

                testIt.Outcome = GetOutcome(result.Properties["Result"].Value as string);
                testIt.ErrorStackTrace = result.Properties["StackTrace"].Value as string;
                testIt.ErrorMessage = result.Properties["FailureMessage"].Value as string;
                testIt.Time = result.Properties["Time"].Value as TimeSpan?;

                if (testIt.Outcome > outcome)
                {
                    outcome = testIt.Outcome;
                }
                if (testIt.Outcome > testContext.Outcome)
                {
                    testContext.Outcome = testIt.Outcome;
                }
            }
            Outcome = outcome;
        }


        private bool HandleParseError(PSObject result, string describe)
        {
            string errorMessage = $"Error in {Path}";
            if (describe.Contains(errorMessage))
            {
                //string stackTraceString = result.Properties["StackTrace"].Value as string;
                //string errorString = result.Properties["FailureMessage"].Value as string;

                SetOutcome(TestOutcome.Failed);

                //foreach (var tc in TestCases)
                //{
                //    var testResult = new TestResult(tc);
                //    testResult.Outcome = TestOutcome.Failed;
                //    testResult.ErrorMessage = errorString;
                //    testResult.ErrorStackTrace = stackTraceString;
                //    _testResults.Add(testResult);
                //}

                return false;
            }

            return true;
        }

        private static TestOutcome GetOutcome(string testResult)
        {
            if (string.IsNullOrEmpty(testResult))
            {
                return TestOutcome.NotFound;
            }

            if (testResult.Equals("passed", StringComparison.OrdinalIgnoreCase))
            {
                return TestOutcome.Passed;
            }
            if (testResult.Equals("skipped", StringComparison.OrdinalIgnoreCase))
            {
                return TestOutcome.Skipped;
            }
            if (testResult.Equals("pending", StringComparison.OrdinalIgnoreCase))
            {
                return TestOutcome.Skipped;
            }
            return TestOutcome.Failed;
        }
    }
}