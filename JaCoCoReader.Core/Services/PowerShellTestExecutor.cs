using System;
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
        private volatile bool _mCancelled;

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
            _mCancelled = false;

            SetupExecutionPolicy();

            solution.SetOutcome(TestOutcome.None);

            RunTestSolutionEx(solution, runContext);
        }

        private void RunTestSolutionEx(TestSolution tests, RunContext runContext)
        {
            foreach (TestProject project in tests.Projects)
            {
                if (_mCancelled)
                {
                    break;
                }
                RunTestFolderEx(project, runContext);
            }
        }

        #endregion

        #region Project Folder

        public void RunTestProject(TestProject project, RunContext runContext)
        {
            _mCancelled = false;

            SetupExecutionPolicy();

            project.SetOutcome(TestOutcome.None);

            RunTestFolderEx(project, runContext);
        }

        public void RunTestFolder(TestFolder folder, RunContext runContext)
        {
            _mCancelled = false;

            SetupExecutionPolicy();

            folder.SetOutcome(TestOutcome.None);

            RunTestFolderEx(folder, runContext);
        }

        private void RunTestFolderEx(TestFolder parentFolder, RunContext runContext)
        {
            foreach (TestFolder folder in parentFolder.Folders)
            {
                if (_mCancelled)
                {
                    break;
                }
                RunTestFolderEx(folder, runContext);
                if (folder.Outcome > parentFolder.Outcome)
                {
                    parentFolder.Outcome = folder.Outcome;
                }
            }

            foreach (TestFile file in parentFolder.Files)
            {
                if (_mCancelled)
                {
                    break;
                }
                RunTestFileEx(file, runContext);
                if (file.Outcome > parentFolder.Outcome)
                {
                    parentFolder.Outcome = file.Outcome;
                }

            }
        }

        #endregion

        #region File

        public void RunTestFile(TestFile file, RunContext runContext)
        {
            _mCancelled = false;

            SetupExecutionPolicy();

            file.SetOutcome(TestOutcome.None);

            RunTestFileEx(file, runContext);
        }

        private void RunTestFileEx(TestFile file, RunContext runContext)
        {
            foreach (TestDescribe describe in file.Describes)
            {
                if (_mCancelled)
                {
                    break;
                }
                RunTestDescribeEx(describe, runContext);
                if (describe.Outcome > file.Outcome)
                {
                    file.Outcome = describe.Outcome;
                }
            }
        }

        #endregion

        public void Cancel()
        {
            _mCancelled = true;
        }

        public void RunTestDescribe(TestDescribe describe, RunContext runContext)
        {
            SetupExecutionPolicy();

            describe.SetOutcome(TestOutcome.None);

            RunTestDescribeEx(describe, runContext);
        }

        private void RunTestDescribeEx(TestDescribe describe, RunContext runContext)
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
                        RunTestDescribeEx(powerShell, describe, runContext);
                    }
                    catch (Exception ex)
                    {
                        describe.SetOutcome(TestOutcome.Failed);
                        //foreach (var testCase in testSet.Contexts)
                        //{
                        //    var testResult = new TestResult(testCase);
                        //    testResult.Outcome = TestOutcome.Failed;
                        //    testResult.ErrorMessage = ex.Message;
                        //    testResult.ErrorStackTrace = ex.StackTrace;
                        //    frameworkHandle.RecordResult(testResult);
                        //}
                    }
                }
            }
        }


        private void RunTestDescribeEx(PowerShell powerShell, TestDescribe describe, RunContext runContext)
        {
            string tempFile = Path.GetTempFileName();
            try
            {

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

                FileInfo fi = new FileInfo(describe.Path);


                powerShell.AddCommand("Invoke-Pester")
                    .AddParameter("Path", fi.Directory?.FullName)
                    .AddParameter("TestName", describe.Name)

                    .AddParameter("DetailedCodeCoverage")
                    .AddParameter("CodeCoverageOutputFile", tempFile)

                    .AddParameter("PassThru");

                Collection<PSObject> pesterResults = powerShell.Invoke();
                powerShell.Commands.Clear();

                // The test results are not necessary stored in the first PSObject.
                Array results = GetTestResults(pesterResults);
                if (results.Length == 0)
                {
                    describe.SetOutcome(TestOutcome.Failed);
                }
                else
                {
                    describe.ProcessTestResults(results);

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
