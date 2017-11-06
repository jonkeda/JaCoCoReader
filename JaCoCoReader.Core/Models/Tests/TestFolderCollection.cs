using System.Collections.ObjectModel;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestFolderCollection : TestModelCollection<TestFolder, TestFolder>
    {
        public TestFolderCollection(TestFolder parent) : base(parent)
        {
        }
    }
}