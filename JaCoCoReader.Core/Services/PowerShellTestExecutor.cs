using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using JaCoCoReader.Core.Models.Tests;
using Microsoft.PowerShell;
using PowerShellTools.TestAdapter;

namespace JaCoCoReader.Core.Services
{
    public class IRunContext
    {
      public  string TestRunDirectory { get; set; }
       public string SolutionDirectory { get; set; }
    }

    public class IFrameworkHandle
    {
        public void RecordResult(object testResult) { }
        public void SendMessage(TestMessageLevel informational, string toString)
        { }
    }

    public class PowerShellTestExecutor
    {
        public const string ExecutorUriString = "executor://PowerShellTestExecutor/v1";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);
        private volatile bool _mCancelled;

        public void RunTestFile(TestFile file, IRunContext runContext,
            IFrameworkHandle frameworkHandle)
        {
            SetupExecutionPolicy();
            //IEnumerable<TestCase> tests = PowerShellTestDiscoverer.GetTests(sources, null);
            RunTestFileEx(file, runContext, frameworkHandle);
        }

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


        private void RunTestFileEx(TestFile tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            _mCancelled = false;
            //SetupExecutionPolicy();

            // var testSets = new List<TestDescribe>();
            //foreach (TestFile testCase in tests)
            //{
            //    var describe = testCase.FullyQualifiedName.Split('.').First();
            //    var codeFile = testCase.CodeFilePath;

            //    var testSet = testSets.FirstOrDefault(m => m.Describe.Equals(describe, StringComparison.OrdinalIgnoreCase) &&
            //                                               m.File.Equals(codeFile, StringComparison.OrdinalIgnoreCase));

            //    if (testSet == null)
            //    {
            //        testSet = new TestDescribe(codeFile, describe);
            //        testSets.Add(testSet);
            //    }

            //    testSet.TestCases.Add(testCase);
            //}

            foreach (var testSet in tests.Describes)
            {
                if (_mCancelled) break;

                StringBuilder testOutput = new StringBuilder();

                try
                {
                    //var testAdapter = new TestAdapterHost();
                    //testAdapter.HostUi.OutputString = s =>
                    //{
                    //    if (!string.IsNullOrEmpty(s))
                    //        testOutput.Append(s);
                    //};

                    Runspace runpsace = RunspaceFactory.CreateRunspace();
                    runpsace.Open();

                    using (PowerShell ps = PowerShell.Create())
                    {
                        ps.Runspace = runpsace;
                        RunTestDescribe(ps, testSet, runContext);

                        foreach (var testResult in testSet.TestResults)
                        {
                            frameworkHandle.RecordResult(testResult);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //foreach (var testCase in testSet.Contexts)
                    //{
                    //    var testResult = new TestResult(testCase);
                    //    testResult.Outcome = TestOutcome.Failed;
                    //    testResult.ErrorMessage = ex.Message;
                    //    testResult.ErrorStackTrace = ex.StackTrace;
                    //    frameworkHandle.RecordResult(testResult);
                    //}
                }

                if (testOutput.Length > 0)
                {
                    frameworkHandle.SendMessage(TestMessageLevel.Informational, testOutput.ToString());
                }
            }
        }

        public void Cancel()
        {
            _mCancelled = true;
        }

        public void RunTestDescribe(PowerShell powerShell, TestDescribe describe, IRunContext runContext)
        {
            SetupExecutionPolicy();

            RunTestDescribeEx(powerShell, describe, runContext);
        }

        private void RunTestDescribeEx(PowerShell powerShell, TestDescribe describe, IRunContext runContext)
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
                .AddParameter("PassThru");

            Collection<PSObject> pesterResults = powerShell.Invoke();
            powerShell.Commands.Clear();

            // The test results are not necessary stored in the first PSObject.
            Array results = GetTestResults(pesterResults);
            describe.ProcessTestResults(results);
        }

        #region

        private string FindModule(string moduleName, IRunContext runContext)
        {
            string pesterPath = GetModulePath(moduleName, runContext.TestRunDirectory);
            if (string.IsNullOrEmpty(pesterPath))
            {
                pesterPath = GetModulePath(moduleName, runContext.SolutionDirectory);
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

        private static string GetModulePath(string moduleName, string root)
        {
            if (root == null)
                return null;

            // Default packages path for nuget.
            string packagesRoot = Path.Combine(root, "packages");

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
