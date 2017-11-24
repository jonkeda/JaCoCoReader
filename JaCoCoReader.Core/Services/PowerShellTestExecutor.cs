using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using JaCoCoReader.Core.Constants;
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

        private PowerShell _powerShell;

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

            foreach (TestFeature feature in parentFolder.Features)
            {
                runContext.TestFeatures.Add(feature.Path, feature);
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

        #region Feature

        public void RunTestFeature(TestFeature feature, RunContext runContext)
        {
            SetupExecutionPolicy();

            feature.SetOutcome(TestOutcome.None);

            runContext.TestFeatures.Add(feature.Path, feature);
            RunTests(runContext);
        }

        #endregion

        public void Cancel()
        {
            try
            {
                _powerShell?.Stop();
            }
            catch
            {
                // don't do anything
            }
        }

        public void RunTestDescribe(TestDescribe describe, RunContext runContext)
        {
            SetupExecutionPolicy();

            describe.SetOutcome(TestOutcome.None);

            runContext.TestFiles.Add(describe.Parent.Path, describe.Parent);
            RunTests(runContext, describe.Name, null);
        }

        public void RunTestScenario(TestScenario scenario, RunContext runContext)
        {
            SetupExecutionPolicy();

            scenario.SetOutcome(TestOutcome.None);

            runContext.TestFeatures.Add(scenario.Parent.Path, scenario.Parent);
            RunTests(runContext, null, scenario.Name);
        }


        private void RunTests(RunContext runContext)
        {
            RunTests(runContext, null, null);
        }

        private void RunTests(RunContext runContext, string describeName, string scenarioName)
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

                using (_powerShell = PowerShell.Create())
                {
                    _powerShell.Runspace = runspace;

                    try
                    {
                        RunTests(describeName, scenarioName, runContext);
                    }
                    catch (Exception ex)
                    {
                        foreach (TestFile file in runContext.TestFiles.Values)
                        {
                            file.SetOutcome(TestOutcome.None);
                        }
                        foreach (TestFeature feature in runContext.TestFeatures.Values)
                        {
                            feature.SetOutcome(TestOutcome.None);
                        }
                    }
                }
            }
        }

        private void RunTests(string describeName, string scenarioName, RunContext runContext)
        {

            runContext.UpdateRunningTest("All tests");

            string module = FindModule("Pester", runContext);
            _powerShell.AddCommand("Import-Module").AddParameter("Name", module);
            _powerShell.Invoke();
            _powerShell.Commands.Clear();

            if (_powerShell.HadErrors)
            {
                ErrorRecord errorRecord = _powerShell.Streams.Error.FirstOrDefault();
                string errorMessage = errorRecord?.ToString() ?? string.Empty;

                throw new Exception($"FailedToLoadPesterModule {errorMessage}");
            }

            RunTestFiles(describeName, runContext);

            RunTestScenarios(scenarioName, runContext);
        }

        private void RunTestScenarios(string scenarioName, RunContext runContext)
        {
            string tempFile = Path.GetTempFileName();
            try
            {
                string[] pathObjects = runContext.TestFeatures.Keys.ToArray();
                if (pathObjects.Length == 0)
                {
                    return;
                }

                _powerShell.AddCommand("Invoke-Gherkin")
                    .AddParameter("Path", pathObjects)
                    //.AddParameter("DetailedCodeCoverage")
                    .AddParameter("CodeCoverageOutputFile", tempFile)
                    .AddParameter("PassThru");
                if (!string.IsNullOrEmpty(scenarioName))
                {
                    _powerShell.AddParameter("ScenarioName", scenarioName);
                }

                string[] codecoverage = GetCodeCoverageFilenames(runContext, name => name.Substring(0, name.Length - Constant.TestsPs1.Length));
                if (codecoverage != null
                    && codecoverage.Length > 0)
                {
                    _powerShell.AddParameter("CodeCoverage", codecoverage);
                }

                Collection<PSObject> pesterResults = _powerShell.Invoke();
                _powerShell.Commands.Clear();

                // The test results are not necessary stored in the first PSObject.
                Array results = GetTestResults(pesterResults);
                if (results.Length == 0)
                {
                    foreach (TestFeature file in runContext.TestFeatures.Values)
                    {
                        file.SetOutcome(TestOutcome.Failed);
                    }
                }
                else
                {
                    int i = 0;
                    TestScenario lastScenario = null;
                    foreach (PSObject result in results)
                    {
                        string filename = result.Properties["Filename"].Value as string;
                        string scenario = result.Properties["Describe"].Value as string;
                        if (filename != null
                            && runContext.TestFeatures.TryGetValue(filename, out TestFeature feature))
                        {
                            TestScenario testScenario = feature.Scenarios.FirstOrDefault(s => s.Name == scenario);
                            if (testScenario != null)
                            { 
                                if (lastScenario != testScenario)
                                {
                                    i = 0;
                                    lastScenario = testScenario;
                                }
                                testScenario.ProcessTestResults(result, i);
                            }
                        }
                        i++;
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


        private void RunTestFiles(string describeName, RunContext runContext)
        {
            string tempFile = Path.GetTempFileName();
            try
            {
                string[] pathObjects = runContext.TestFiles.Keys.ToArray();
                if (pathObjects.Length == 0)
                {
                    return;
                }

                _powerShell.AddCommand("Invoke-Pester")
                    .AddParameter("Path", pathObjects)
                    .AddParameter("DetailedCodeCoverage")
                    .AddParameter("CodeCoverageOutputFile", tempFile)
                    .AddParameter("PassThru");
                if (!string.IsNullOrEmpty(describeName))
                {
                    _powerShell.AddParameter("TestName", describeName);
                }


                string[] codecoverage = GetCodeCoverageFilenames(runContext, name => name.Substring(0, name.Length - Constant.TestsPs1.Length));
                if (codecoverage != null
                    && codecoverage.Length > 0)
                {
                    _powerShell.AddParameter("CodeCoverage", codecoverage);
                }

                Collection<PSObject> pesterResults = _powerShell.Invoke();
                _powerShell.Commands.Clear();

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

        private string[] GetCodeCoverageFilenames(RunContext context, Func<string, string> getName)
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
                            string name = getName(Path.GetFileName(testfile.Path));
                            string filename = name + Constant.Ps1;
                            if (!testScriptFilenames.ContainsKey(filename))
                            {
                                testScriptFilenames.Add(filename, testfile.Path);
                            }
                            filename = name + Constant.Psm1;
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
