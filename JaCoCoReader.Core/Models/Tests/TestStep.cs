using Gherkin.Ast;
using JaCoCoReader.Core.UI.Icons;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestStep : TestFileModel<TestScenario>
    {
        public override string Type
        {
            get { return "Step"; }
        }

        public override FontAwesomeIcon Icon
        {
            get { return FontAwesomeIcon.StepForward; }
        }

        public Step Step { get; set; }

        public override void CalculateOutcome()
        {

        }

        protected override void DoMerge(TestModel model)
        {
        }
    }
}