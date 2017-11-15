using System.Collections.Generic;
using JaCoCoReader.Core.UI.Icons;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestContext : TestFileModel<TestDescribe>
    {
        public TestContext()
        {
            Its = new TestItCollection(this);
        }

        public TestItCollection Its { get; }

        public override IEnumerable<TestModel> Items
        {
            get { return Its; }
        }

        public override string Type
        {
            get { return "Context"; }
        }

        public override FontAwesomeIcon Icon
        {
            get { return FontAwesomeIcon.Reorder; }
        }

    }
}