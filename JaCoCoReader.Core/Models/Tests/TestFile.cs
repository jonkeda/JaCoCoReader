using System.Collections.Generic;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestFile : TestFileModel<TestFolder>
    {
        public TestFile()
        {
            Describes = new TestDescribeCollection(this);
        }

        public TestDescribeCollection Describes { get; }

        public override string Type
        {
            get { return "File"; }
        }

        public override IEnumerable<TestModel> Items
        {
            get { return Describes; }
        }


    }
}