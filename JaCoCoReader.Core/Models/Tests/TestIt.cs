using System.Management.Automation.Language;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestIt : TestFileModel<TestContext>
    {
        public CommandAst Ast { get; set; }

        public override string Type
        {
            get { return "It"; }
        }

    }
}