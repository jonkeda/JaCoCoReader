using System.Collections.Generic;
using Gherkin.Ast;
using JaCoCoReader.Core.Services;
using JaCoCoReader.Core.UI.Icons;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestFeature : TestFileModel<TestFolder>
    {
        public TestFeature()
        {
            Scenarios = new TestScenarioCollection(this);
        }

        public TestScenarioCollection Scenarios { get; }

        private TestModelByLine _testModelByLine;

        public override string Type
        {
            get { return "File"; }
        }

        public override FontAwesomeIcon Icon
        {
            get { return FontAwesomeIcon.Film; }
        }

        public override IEnumerable<TestModel> Items
        {
            get { return Scenarios; }
        }

        public GherkinDocument Document { get; set; }

        protected override void DoMerge(TestModel model)
        {
            if (model is TestFeature testFiles)
            {
                _testModelByLine = null;

                Scenarios.Merge(testFiles.Scenarios);
            }
        }

        public TestOutcome GetOutcome(int line)
        {
            if (_testModelByLine == null)
            {
                TestModelByLine testModelByLine = new TestModelByLine();
                GetLineNumbers(this, testModelByLine);
                _testModelByLine = testModelByLine;
            }

            if (_testModelByLine.TryGetValue(line, out TestModel model))
            {
                return model.Outcome;
            }
            return TestOutcome.None;
        }

        private void GetLineNumbers(TestModel model, TestModelByLine byLine)
        {
            foreach (TestModel item in model.Items)
            {
                if (!byLine.ContainsKey(item.LineNr))
                {
                    byLine.Add(item.LineNr, item);
                }
                GetLineNumbers(item, byLine);
            }
        }
    }
}