using System;
using JaCoCoReader.Models;

namespace JaCoCoReader.ViewModels
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