using System.Collections.Generic;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestFolder : TestModel
    {
        public TestFolderCollection Folders { get; } = new TestFolderCollection();

        public TestFileCollection Files { get; } = new TestFileCollection();

        public string Path { get; set; }

        public override IEnumerable<TestModel> Items
        {
            get
            {
                foreach (TestFolder folder in Folders)
                {
                    yield return folder;
                }
                foreach (TestFile file in Files)
                {
                    yield return file;
                }
            }
        }
    }
}