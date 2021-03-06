﻿using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using JaCoCoReader.Core.Constants;
using JaCoCoReader.Core.Models.Tests;
using JaCoCoReader.Core.Services;
using JaCoCoReader.Vsix.Extensions;

namespace JaCoCoReader.Vsix.Services
{
    public class VsPowerShellTestDiscoverer : PowerShellTestDiscoverer
    {
        internal static IEnumerable<TestProject> GetTests()
        {
            foreach (Project vsProject in VsExtensions.GetProjects())
            {
                TestProject project = CreateProject(vsProject);
                if (project != null)
                {
                    yield return project;
                }
            }
        }

        internal static TestProject CreateProject(Project vsProject)
        {
            TestProject project = new TestProject
            {
                Name = vsProject.Name,
                Path = vsProject.FileName
            };


            GetVsTests(vsProject.ProjectItems, project);
            if (project.Folders.Count > 0
                || project.Files.Count > 0)
            {
                return project;
            }
            return null;
        }

        private static void GetVsTests(ProjectItems vsProjectProjectItems, TestFolder parentFolder)
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
                    TestFolder folder = new TestFolder
                    {
                        Name = item.Name
                    };

                    GetVsTests(item.ProjectItems, folder);

                    if (folder.Folders.Count > 0
                        || folder.Files.Count > 0)
                    {
                        parentFolder.Folders.Add(folder);
                    }
                }
                else
                {
                    for (short i = 1; i <= item.FileCount; i++)
                    {
                        string fileName = item.FileNames[i];
                        if (fileName.EndsWith(Constant.TestsPs1, StringComparison.InvariantCultureIgnoreCase))
                        {
                            TestFile file = new TestFile
                            {
                                Name = item.Name,
                                Path = fileName
                            };

                            if (DiscoverPesterTests(fileName, file.Describes, null))
                            {
                                if (file.Describes.Count > 0)
                                {
                                    parentFolder.Files.Add(file);
                                }
                            }
                        }
                        else if (fileName.EndsWith(Constant.Feature, StringComparison.InvariantCultureIgnoreCase))
                        {
                            TestFeature feature = DiscoverPesterFeatures(fileName, null);

                            if (feature != null)
                            {
                                parentFolder.Features.Add(feature);
                            }
                        }
                    }
                }
            }
        }


    }
}