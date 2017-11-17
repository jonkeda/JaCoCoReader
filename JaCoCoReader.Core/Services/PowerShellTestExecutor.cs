using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using JaCoCoReader.Core.Models.CodeCoverage;
using JaCoCoReader.Core.Models.Tests;
using Microsoft.PowerShell;

namespace JaCoCoReader.Core.Services
{
    public class PowerShellTestExecutor
    {
        public const string ExecutorUriString = "executor://PowerShellTestExecutor/v1";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);

        #region Setup Execution Policy

        private static void SetupExecutionPolicy()
        {
            SetExecutionPolicy(ExecutionPolicy.RemoteSigned, ExecutionPolicyScope.Process);
        }

        private static void SetExecutionPolicy(ExecutionPolicy policy, ExecutionPolicyScope scope)
        {
            ExecutionPolicy currentPolicy = ExecutionPolicy.Undefined;

            using (PowerShell ps = PowerShell.Create())
            {
                ps.AddCommand("Get-ExecutionPolicy");

                foreach (PSObject result in ps.Invoke())
                {
                    currentPolicy = ((ExecutionPolicy)result.BaseObject);
                    break;
                }

                if ((currentPolicy <= policy || currentPolicy == ExecutionPolicy.Bypass) && currentPolicy != ExecutionPolicy.Undefined) //Bypass is the absolute least restrictive, but as added in PS 2.0, and thus has a value of '4' instead of a value that corresponds to it's relative restrictiveness
                    return;

                ps.Commands.Clear();

                ps.AddCommand("Set-ExecutionPolicy").AddParameter("ExecutionPolicy", policy).AddParameter("Scope", scope).AddParameter("Force");
                ps.Invoke();
            }
        }


        #endregion

        #region TestSolution

        public void RunTestSolution(TestSolution solution, RunContext runContext)
        {
            SetupExecutionPolicy();

            solution.SetOutcome(TestOutcome.None);

            RunTestSolutionEx(solution, runContext);

            RunTests(runContext);
        }

        private void RunTestSolutionEx(TestSolution tests, RunContext runContext)
        {
            foreach (TestProject project in tests.Projects)
            {
                CollectTestFolder(project, runContext);
            }
        }

        #endregion

        #region Project Folder

        public void RunTestProject(TestProject project, RunContext runContext)
        {
            SetupExecutionPolicy();

            project.SetOutcome(TestOutcome.None);

            CollectTestFolder(project, runContext);
            RunTests(runContext);
        }

        public void RunTestFolder(TestFolder folder, RunContext runContext)
        {
            SetupExecutionPolicy();

            folder.SetOutcome(TestOutcome.None);

            CollectTestFolder(folder, runContext);
            RunTests(runContext);
        }

        private void CollectTestFolder(TestFolder parentFolder, RunContext runContext)
        {
            foreach (TestFolder folder in parentFolder.Folders)
            {
                CollectTestFolder(folder, runContext);
            }

            foreach (TestFile file in parentFolder.Files)
            {
                runContext.TestFiles.Add(file.Path, file);
            }
        }

        #endregion

        #region File

        public void RunTestFile(TestFile file, RunContext runContext)
        {
            SetupExecutionPolicy();

            file.SetOutcome(TestOutcome.None);

            runContext.TestFiles.Add(file.Path, file);
            RunTests(runContext);
        }

        #endregion

        public void Cancel()
        {
            //_mCancelled = true;
        }

        public void RunTestDescribe(TestDescribe describe, RunContext runContext)
        {
            SetupExecutionPolicy();

            describe.SetOutcome(TestOutcome.None);

            runContext.TestFiles.Add(describe.Parent.Path, describe.Parent);
            RunTests(runContext);
        }

        private void RunTests(RunContext runContext)
        {
            RunTests(runContext, null);
        }

        private void RunTests(RunContext runContext, string describeName)
        {
            StringBuilder testOutput = new StringBuilder();
            TestAdapterHost testAdapter = new TestAdapterHost();
            testAdapter.HostUi.OutputString = s =>
            {
                if (!string.IsNullOrEmpty(s))
                {
                    testOutput.Append(s);
                }
            };

            using (Runspace runspace = RunspaceFactory.CreateRunspace(testAdapter))
            {
                runspace.Open();

                using (PowerShell powerShell = PowerShell.Create())
                {
                    powerShell.Runspace = runspace;

                    try
                    {
                        RunTests(powerShell, describeName, runContext);

                        //describe.Output = testOutput.ToString();
                    }
                    catch (Exception)
                    {
                        foreach (TestFile file in runContext.TestFiles.Values)
                        {
                            file.SetOutcome(TestOutcome.Failed);
                        }
  }
                }
            }
        }

        private void RunTests(PowerShell powerShell, string describeName, RunContext runContext)
        {
            string tempFile = Path.GetTempFileName();
            try
            {
                runContext.UpdateRunningTest("All tests");

                string module = FindModule("Pester", runContext);
                powerShell.AddCommand("Import-Module").AddParameter("Name", module);
                powerShell.Invoke();
                powerShell.Commands.Clear();

                if (powerShell.HadErrors)
                {
                    ErrorRecord errorRecord = powerShell.Streams.Error.FirstOrDefault();
                    string errorMessage = errorRecord?.ToString() ?? string.Empty;

                    throw new Exception($"FailedToLoadPesterModule {errorMessage}");
                }

                string[] pathObjects = runContext.TestFiles.Keys.ToArray();


                powerShell.AddCommand("Invoke-Pester")
                    .AddParameter("Path", pathObjects)
                    .AddParameter("DetailedCodeCoverage")
                    .AddParameter("CodeCoverageOutputFile", tempFile)

                    .AddParameter("PassThru");
                if (!string.IsNullOrEmpty(describeName))
                {
                    powerShell.AddParameter("TestName", describeName);
                }


                string[] codecoverage = GetCodeCoverageFilenames(runContext);
                if (codecoverage != null
                    && codecoverage.Length > 0)
                {
                    powerShell.AddParameter("CodeCoverage", codecoverage);
                }

                Collection<PSObject> pesterResults = powerShell.Invoke();
                powerShell.Commands.Clear();

                // The test results are not necessary stored in the first PSObject.
                Array results = GetTestResults(pesterResults);
                if (results.Length == 0)
                {
                    foreach (TestFile file in runContext.TestFiles.Values)
                    {
                        file.SetOutcome(TestOutcome.Failed);
                    }
                }
                else
                {
                    foreach (PSObject result in results)
                    {
                        string filename = result.Properties["Filename"].Value as string;
                        string describe = result.Properties["Describe"].Value as string;
                        if (filename != null
                            && runContext.TestFiles.TryGetValue(filename, out TestFile file))
                        {
                            foreach (TestDescribe testDescribe in file.Describes)
                            {
                                if (testDescribe.Name == describe)
                                {
                                    testDescribe.ProcessTestResults(result);
                                }
                            }
                        }
                    }
                    Report report = Report.Load(tempFile);
                    if (report != null)
                    {
                        runContext.AddCodeCoverageReport(report);
                    }
                }
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        private string[] GetCodeCoverageFilenames(RunContext context)
        {
            switch (context.CoveredScripts)
            {
                case CoveredScripts.AllScripts:
                    {
                        return context.ScriptFileNames.ToArray();
                    }
                case CoveredScripts.FromDescribeParameter:
                    {
                        return null;
                    }
                case CoveredScripts.SelectedScript:
                    {
                        return null;
                    }
                case CoveredScripts.SameNamedScripts:
                    {
                        Dictionary<string, string> testScriptFilenames = new Dictionary<string, string>();

                        foreach (TestFile testfile in context.TestFiles.Values)
                        {
                            string name = Path.GetFileName(testfile.Path);
                            string filename = name.Substring(0, name.Length - ".tests.ps1".Length) + ".ps1";
                            if (!testScriptFilenames.ContainsKey(filename))
                            {
                                testScriptFilenames.Add(filename, testfile.Path);
                            }
                        }
                        List<string> coverageFilenames = new List<string>();
                        foreach (string fileName in context.ScriptFileNames)
                        {
                            string name = Path.GetFileName(fileName);
                            if (testScriptFilenames.ContainsKey(name))
                            {
                                coverageFilenames.Add(fileName);
                            }
                        }
                        return coverageFilenames.ToArray();
                    }
            }
            return null;
        }

        #region

        private string FindModule(string moduleName, RunContext runContext)
        {
            string pesterPath = GetPesterModulePath(moduleName, Path.GetDirectoryName(typeof(PowerShellTestExecutor).Assembly.Location), "Pester");

            if (string.IsNullOrEmpty(pesterPath))
            {
                pesterPath = GetModulePath(moduleName, runContext.TestRunDirectory, "packages");
            }
            if (string.IsNullOrEmpty(pesterPath))
            {
                pesterPath = GetModulePath(moduleName, runContext.SolutionDirectory, "packages");
            }

            if (string.IsNullOrEmpty(pesterPath))
            {
                pesterPath = moduleName;
            }

            return pesterPath;
        }

        /// <summary>
        /// Gets test results from the <see cref="PSObject"/> collection.
        /// </summary>
        /// <param name="psObjects">
        /// The <see cref="PSObject"/> collection as returned from the <c>Invoke-Pester</c> command
        /// </param>
        /// <returns>
        /// The test results as <see cref="Array"/>
        /// </returns>
        private static Array GetTestResults(Collection<PSObject> psObjects)
        {
            PSObject resultObject = psObjects.FirstOrDefault(o => o.Properties["TestResult"] != null);

            return resultObject?.Properties["TestResult"].Value as Array;
        }

        private static string GetPesterModulePath(string moduleName, string root, string folder)
        {
            if (root == null)
                return null;

            // Default packages path for nuget.
            string packagesRoot = Path.Combine(root, folder);

            if (Directory.Exists(packagesRoot))
            {
                string psm1 = Path.Combine(packagesRoot, $@"{moduleName}.psm1");
                if (File.Exists(psm1))
                {
                    return psm1;
                }
            }

            return null;
        }

        private static string GetModulePath(string moduleName, string root, string folder)
        {
            if (root == null)
                return null;

            // Default packages path for nuget.
            string packagesRoot = Path.Combine(root, folder);

            // TODO: Scour for custom nuget packages paths.

            if (Directory.Exists(packagesRoot))
            {
                string packagePath = Directory.GetDirectories(packagesRoot, moduleName + "*", SearchOption.TopDirectoryOnly).FirstOrDefault();
                if (null != packagePath)
                {
                    string psd1 = Path.Combine(packagePath, $@"tools\{moduleName}.psd1");
                    if (File.Exists(psd1))
                    {
                        return psd1;
                    }

                    string psm1 = Path.Combine(packagePath, $@"tools\{moduleName}.psm1");
                    if (File.Exists(psm1))
                    {
                        return psm1;
                    }
                    string dll = Path.Combine(packagePath, $@"tools\{moduleName}.dll");
                    if (File.Exists(dll))
                    {
                        return dll;
                    }
                }
            }

            return null;
        }

        #endregion
    }

}
