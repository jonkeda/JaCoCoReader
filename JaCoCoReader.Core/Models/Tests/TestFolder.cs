using System.Collections.Generic;
using JaCoCoReader.Core.UI.Icons;

namespace JaCoCoReader.Core.Models.Tests
{
    public class TestFolder : TestModel<TestFolder>
    {
        public TestFolder()
        {
            Folders = new TestFolderCollection(this);
            Files = new TestFileCollection(this);
        }

        public TestFolderCollection Folders { get; } 

        public TestFileCollection Files { get; }

        public string Path { get; set; }

        public override string Type
        {
            get { return "Folder"; }
        }

        public override FontAwesomeIcon Icon
        {
            get { return FontAwesomeIcon.Folder; }
        }

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