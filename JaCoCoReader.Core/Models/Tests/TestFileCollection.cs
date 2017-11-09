namespace JaCoCoReader.Core.Models.Tests
{
    public class TestFileCollection : TestModelCollection<TestFile, TestFolder>
    {
        public TestFileCollection(TestFolder parent) : base(parent)
        {
        }
    }
}