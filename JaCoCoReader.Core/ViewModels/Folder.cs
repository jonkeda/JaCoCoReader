using JaCoCoReader.Core.Models;

namespace JaCoCoReader.Core.ViewModels
{
    public class Folder
    {
        public Folder(string name)
        {
            Name = name;
        }

        public string Name { get; set; }


        public FolderCollection Folders { get; } = new FolderCollection();

        public SourcefileCollection Sourcefiles { get; } = new SourcefileCollection();

        public int MissedLines { get; set; }

        public int CoveredLines { get; set; }
    }
}