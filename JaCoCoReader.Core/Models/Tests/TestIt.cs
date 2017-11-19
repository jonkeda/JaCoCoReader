using System.Management.Automation.Language;
using JaCoCoReader.Core.UI.Icons;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestIt : TestFileModel<TestContext>
    {
        public CommandAst Ast { get; set; }

        public override string Type
        {
            get { return "It"; }
        }

        public override FontAwesomeIcon Icon
        {
            get { return FontAwesomeIcon.Check; }
        }

        public override void CalculateOutcome()
        {
            
        }

        protected override void DoMerge(TestModel model)
        {
        }
    }
}