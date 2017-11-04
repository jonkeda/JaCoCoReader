using System.Collections.Generic;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestContext : TestFileModel
    {
        public TestItCollection Its { get; } = new TestItCollection();

        public override IEnumerable<TestModel> Items
        {
            get { return Its; }
        }
    }
}