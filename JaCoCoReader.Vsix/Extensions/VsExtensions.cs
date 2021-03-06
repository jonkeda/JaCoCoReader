﻿using System;
using System.Collections.Generic;
using System.IO;
using EnvDTE;
using JaCoCoReader.Core.Constants;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace JaCoCoReader.Vsix.Extensions
{
    public static class VsExtensions
    {
        public static bool IsFolder()
        {
            DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));

            string extension = Path.GetExtension(dte.Solution.FullName);

            return string.IsNullOrEmpty(extension);
        }

        public static IEnumerable<Project> GetProjects()
        {
            IVsSolution solution = (IVsSolution)Package.GetGlobalService(typeof(IVsSolution));
            foreach (Project project in GetProjects(solution))
            {
                yield return project;
            }
        }

        public static IVsSolution GetSolution()
        {
            return (IVsSolution)Package.GetGlobalService(typeof(IVsSolution));
        }

        public static IEnumerable<Project> GetProjects(this IVsSolution solution)
        {
            foreach (IVsHierarchy hier in GetProjectsInSolution(solution))
            {
                Project project = GetDteProject(hier);
                if (project != null)
                {
                    yield return project;
                }
            }
        }

        public static IEnumerable<IVsHierarchy> GetProjectsInSolution(this IVsSolution solution)
        {
            return GetProjectsInSolution(solution, __VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION);
        }

        public static IEnumerable<IVsHierarchy> GetProjectsInSolution(this IVsSolution solution, __VSENUMPROJFLAGS flags)
        {
            if (solution == null)
            {
                yield break;
            }

            Guid guid = Guid.Empty;
            solution.GetProjectEnum((uint)flags, ref guid, out IEnumHierarchies enumHierarchies);
            if (enumHierarchies == null)
            {
                yield break;
            }

            IVsHierarchy[] hierarchy = new IVsHierarchy[1];
            while (enumHierarchies.Next(1, hierarchy, out uint fetched) == VSConstants.S_OK && fetched == 1)
            {
                if (hierarchy.Length > 0 && hierarchy[0] != null)
                {
                    yield return hierarchy[0];
                }
            }
        }

        public static Project GetDteProject(this IVsHierarchy hierarchy)
        {
            if (hierarchy == null)
            {
                throw new ArgumentNullException(nameof(hierarchy));
            }

            hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out object obj);
            return obj as Project;
        }

        public static ProjectItem FindProjectItem(string filename)
        {
            DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));

            return dte.Solution.FindProjectItem(filename);
        }

        public static void OpenProjectItem(string filename)
        {
            ProjectItem item = FindProjectItem(filename);
            if (item != null)
            {
                Window window = item.Open(Constant.vsViewKindTextView);
                if (item.IsOpen[Constant.vsViewKindTextView])
                {
                    window.Activate();
                }
                else
                {
                    window.Visible = true;
                }
            }
        }

        public static void SaveProjectItem(string filename)
        {
            ProjectItem item = FindProjectItem(filename);
            item?.Save();
        }
    }
}
