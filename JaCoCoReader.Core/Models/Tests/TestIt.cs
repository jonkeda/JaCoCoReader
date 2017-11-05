using System.Management.Automation.Language;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestIt : TestFileModel
    {
        public CommandAst Ast { get; set; }
        public string ErrorStackTrace { get; set; }
        public string ErrorMessage { get; set; }
    }
}