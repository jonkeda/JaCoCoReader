using System.Collections.Generic;

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
    }
}