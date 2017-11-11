using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using EnvDTE;
using JaCoCoReader.Core.Models.Tests;
using JaCoCoReader.Core.ViewModels.CodeCoverage;
using JaCoCoReader.Core.ViewModels.Tests;
using JaCoCoReader.Vsix.Extensions;
using JaCoCoReader.Vsix.Services;

namespace JaCoCoReader.Vsix.ViewModels
{
    public class VsTestsViewModel : TestsViewModel
    {
        public VsTestsViewModel(CodeCoverageViewModel report)
            : base(report)
        { }

        public override Visibility RefreshVisibility
        {
            get { return Visibility.Collapsed; }
        }

        public override Visibility LoadVisibility
        {
            get { return Visibility.Visible; }
        }

        protected override void DoLoadCommand()
        {
            LoadFromSolution();

            if (Model.Projects.Count == 0)
            {
                base.DoLoadCommand();
            }
        }

        protected void LoadFromSolution()
        {
            Model.Projects.Clear();

            foreach (TestProject testProject in VsPowerShellTestDiscoverer.GetTests())
            {
                Model.Projects.Add(testProject);
            }
        }

        public override List<string> GetScriptFileNames()
        {
            List<string> fileNames = new List<string>();
            foreach (Project vsProject in VsExtensions.GetProjects())
            {
                GetScriptFileNames(vsProject.ProjectItems, fileNames);
            }
            if (fileNames.Count == 0)
            {
                return base.GetScriptFileNames();
            }
            return fileNames;
        }

        private static void GetScriptFileNames(ProjectItems vsProjectProjectItems, List<string> fileNames)
        {
            if (vsProjectProjectItems == null)
            {
                return;
            }
            foreach (Project​Item item in vsProjectProjectItems.OfType<Project​Item>())
            {
                if (item.ProjectItems != null
                    && item.ProjectItems.Count > 0)
                {
                    GetScriptFileNames(item.ProjectItems, fileNames);
                }
                else
                {
                    for (short i = 1; i <= item.FileCount; i++)
                    {
                        string fileName = item.FileNames[i];
                        if (fileName.EndsWith(".ps1", StringComparison.InvariantCultureIgnoreCase)
                            || fileName.EndsWith(".psm1", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (!fileName.EndsWith(".tests.ps1", StringComparison.InvariantCultureIgnoreCase)
                            || !fileName.EndsWith(".tests.psm1", StringComparison.InvariantCultureIgnoreCase))
                            {
                                fileNames.Add(fileName);
                            }
                        }
                    }
                }
            }
        }

        protected override void DoOpenFileCommand()
        {
            if (SelectedNode is ITestFileModel file)
            {
                VsExtensions.OpenProjectItem(file.Path);
            }
        }
    }
}
