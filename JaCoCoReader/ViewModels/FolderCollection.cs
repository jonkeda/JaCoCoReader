using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JaCoCoReader.Models;
using JaCoCoReader.UI;

namespace JaCoCoReader.ViewModels
{
    public class FolderCollection : Collection<Folder>
    {
        public void Set(IEnumerable<Sourcefile> sourcefiles)
        {
            Folder root = new Folder("root");
            Add(root);
            foreach (Sourcefile file in sourcefiles.OrderBy(p => p.Name))
            {
                Folder currentFolder = root;
                var parts = file.Name.Split('\\');
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    string key = parts[i];
                    Folder branch = currentFolder.Folders.FirstOrDefault(p => p.Name == key);
                    if (branch == null)
                    {
                        branch = new Folder(key);
                        currentFolder.Folders.Add(branch);
                    }
                    currentFolder = branch;
                }
                currentFolder.Sourcefiles.Add(file);
            }

            CountLines(root);
        }

        private void CountLines(Folder root)
        {
            int missedLines = 0;
            int coveredLines = 0;
            foreach (var folder in root.Folders)
            {
                CountLines(folder);
                missedLines += folder.MissedLines;
                coveredLines += folder.CoveredLines;
            }
            foreach (Sourcefile sourcefile in root.Sourcefiles)
            {
                Counter counter = sourcefile.Counters.FirstOrDefault(c => c.Type == CounterType.Line);
                if (counter != null)
                {
                    missedLines += counter.Missed;
                    coveredLines += counter.Covered;
                }
            }
            root.MissedLines = missedLines;
            root.CoveredLines = coveredLines;
        }
    }
}