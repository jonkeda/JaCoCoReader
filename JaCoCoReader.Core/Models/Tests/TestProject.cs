using JaCoCoReader.Core.UI.Icons;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestProject : TestFolder
    {
        public override string Type
        {
            get { return "Project"; }
        }

        public override FontAwesomeIcon Icon
        {
            get { return FontAwesomeIcon.List; }
        }
    }
}