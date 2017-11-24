using System;
using System.Collections.Generic;
using System.Management.Automation;
using Gherkin.Ast;
using JaCoCoReader.Core.Services;
using JaCoCoReader.Core.UI.Icons;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestScenario : TestFileModel<TestFeature>
    {
        public TestScenario()
        {
            Steps = new TestStepCollection(this);
        }

        public TestStepCollection Steps { get; }

        //public Ast Ast { get; set; }

        public override string Type
        {
            get { return "Scenario"; }
        }

        public override FontAwesomeIcon Icon
        {
            get { return FontAwesomeIcon.Image; }
        }

        public ScenarioDefinition Scenario { get; set; }

        public override IEnumerable<TestModel> Items
        {
            get { return Steps; }
        }

        public void ProcessTestResults(PSObject result, int index)
        {
            TestOutcome outcome = TestOutcome.None;
            string scenario = result.Properties["Describe"].Value as string;
            if (!HandleParseError(result, scenario))
            {
                return;
            }

            if (index < Steps.Count)
            {
                string name = result.Properties["Name"].Value as string;

                //// Skip test cases we aren't trying to run
                TestStep testStep = Steps[index]; // testContext?.Its.FirstOrDefault(m => m.Name == name);
                if (testStep == null)
                {
                    return;
                }

                testStep.Outcome = GetOutcome(result.Properties["Result"].Value as string);
                testStep.ErrorStackTrace = result.Properties["StackTrace"].Value as string;
                testStep.ErrorMessage = result.Properties["FailureMessage"].Value as string;
                testStep.Time = result.Properties["Time"].Value as TimeSpan?;

                if (testStep.Outcome > outcome)
                {
                    outcome = testStep.Outcome;
                }
            }
            Outcome = outcome;
        }


        private bool HandleParseError(PSObject result, string scenario)
        {
            string errorMessage = $"Error in {Path}";
            if (scenario.Contains(errorMessage))
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

        protected override void DoMerge(TestModel model)
        {
            if (model is TestScenario testScenario)
            {
                Steps.Merge(testScenario.Steps);
            }
        }

    }
}