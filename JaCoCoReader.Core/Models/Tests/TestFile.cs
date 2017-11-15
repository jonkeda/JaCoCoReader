using System.Collections.Generic;
using JaCoCoReader.Core.UI.Icons;

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

        public override FontAwesomeIcon Icon
        {
            get { return FontAwesomeIcon.FileText; }
        }

        public override IEnumerable<TestModel> Items
        {
            get { return Describes; }
        }


    }
}