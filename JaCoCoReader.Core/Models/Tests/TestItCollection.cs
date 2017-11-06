using System.Collections.ObjectModel;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestItCollection : TestModelCollection<TestIt, TestContext>
    {
        public TestItCollection(TestContext parent) : base(parent)
        {
        }
    }
}