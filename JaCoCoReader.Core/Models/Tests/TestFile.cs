using System.Collections.Generic;
using JaCoCoReader.Core.Services;
using JaCoCoReader.Core.UI.Icons;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestFile : TestFileModel<TestFolder>
    {
        public TestFile()
        {
            Describes = new TestDescribeCollection(this);
        }

        public TestDescribeCollection Describes { get; }

        private TestModelByLine _testModelByLine;

        public override string Type
        {
            get { return "File"; }
        }

        public override FontAwesomeIcon Icon
        {
            get { return FontAwesomeIcon.FileText; }
        }

        public override IEnumerable<TestModel> Items
        {
            get { return Describes; }
        }

        protected override void DoMerge(TestModel model)
        {
            if (model is TestFile testFiles)
            {
                _testModelByLine = null;

                Describes.Merge(testFiles.Describes);
            }
        }

        public TestOutcome? GetOutcome(int line)
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
            return null;
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