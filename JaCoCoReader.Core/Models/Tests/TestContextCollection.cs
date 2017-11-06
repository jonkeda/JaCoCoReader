using System.Collections.ObjectModel;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestContextCollection : TestModelCollection<TestContext, TestDescribe>
    {
        public TestContextCollection(TestDescribe parent) : base(parent)
        {
        }
    }
}