﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using EnvDTE;
using JaCoCoReader.Core.Constants;
using JaCoCoReader.Core.Models.Tests;
using JaCoCoReader.Core.ViewModels.CodeCoverage;
using JaCoCoReader.Core.ViewModels.Tests;
using JaCoCoReader.Vsix.Extensions;
using JaCoCoReader.Vsix.Services;
using Microsoft.VisualStudio.Shell;

namespace JaCoCoReader.Vsix.ViewModels
{
    public class VsTestsViewModel : TestsViewModel
    {
        public VsTestsViewModel(CodeCoverageViewModel report)
            : base(report)
        {
            DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));

            dte.Events.SolutionEvents.Opened += SolutionEventsOnOpened;
            dte.Events.SolutionEvents.ProjectAdded += SolutionEventsOnProjectAdded;
        }

        private void SolutionEventsOnProjectAdded(Project project)
        {
            TestProject testProject = VsPowerShellTestDiscoverer.CreateProject(project);
            if (testProject != null)
            {
                TestSolution solution = new TestSolution();
                solution.Projects.Add(testProject);
                Model.Merge(solution);
            }
        }

        private void SolutionEventsOnOpened()
        {
            LoadFromSolution();
        }

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
            if (VsExtensions.IsFolder())
            {
                DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));

                LoadFromFolder(dte.Solution.FileName);
            }
            else
            {
                LoadFromSolution();
            }

            if (Model.Projects.Count == 0)
            {
                base.DoLoadCommand();
            }
        }

        protected void LoadFromSolution()
        {
            //Model.Projects.Clear();

            foreach (TestProject testProject in VsPowerShellTestDiscoverer.GetTests())
            {
                TestSolution solution = new TestSolution();
                solution.Projects.Add(testProject);
                Model.Merge(solution);
                CleartestFilesByPath();
                OnModelChanged();

                //Model.Projects.Add(testProject);
            }
        }

        public override List<string> GetScriptFileNames()
        {
            if (VsExtensions.IsFolder())
            {
                return base.GetScriptFileNames();
            }

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
                        if (fileName.EndsWith(Constant.Ps1, StringComparison.InvariantCultureIgnoreCase)
                            || fileName.EndsWith(Constant.Psm1, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (!fileName.EndsWith(Constant.TestsPs1, StringComparison.InvariantCultureIgnoreCase)
                                && !fileName.EndsWith(Constant.StepsPs1, StringComparison.InvariantCultureIgnoreCase))
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
