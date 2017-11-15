using System.Collections.Generic;
using JaCoCoReader.Core.UI.Icons;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestSolution : TestModel
    {
        public TestProjectCollection Projects { get; } = new TestProjectCollection();

        public override string Type
        {
            get { return "Solution"; }
        }

        public override FontAwesomeIcon Icon
        {
            get { return FontAwesomeIcon.List; }
        }

        public override IEnumerable<TestModel> Items
        {
            get { return Projects; }
        }
    }
}