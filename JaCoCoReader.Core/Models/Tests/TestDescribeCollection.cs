using System.Collections.ObjectModel;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestDescribeCollection : TestModelCollection<TestDescribe, TestFile>
    {
        public TestDescribeCollection(TestFile parent) : base(parent)
        {
        }
    }
}