using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using JaCoCoReader.Core.Constants;
using JaCoCoReader.Core.Models.Tests;
using JaCoCoReader.Core.Services;
using JaCoCoReader.Core.Threading;
using JaCoCoReader.Core.UI;
using JaCoCoReader.Core.ViewModels.CodeCoverage;

namespace JaCoCoReader.Core.ViewModels.Tests
{
    public class TestsViewModel : ModelViewModel<TestSolution>
    {
        private readonly CodeCoverageViewModel _codeCoverage;

        public TestsViewModel(CodeCoverageViewModel codeCoverage)
        {
            _codeCoverage = codeCoverage;
        }

        private TestModel _selectedNode;
        private bool _running;
        private string _runningTest;
        private TestFilesByPath _testFilesByPath;

        public TestModel SelectedNode
        {
            get { return _selectedNode; }
            set { SetProperty(ref _selectedNode, value); }
        }

        public bool ClearCodeCoverage
        {
            get { return _clearCodeCoverage; }
            set { SetProperty(ref _clearCodeCoverage, value); }
        }

        public virtual Visibility RefreshVisibility
        {
            get { return Visibility.Collapsed; }
        }

        public ICommand RefreshCommand
        {
            get { return new TargetCommand(DoRefreshCommand); }
        }

        public virtual Visibility LoadVisibility
        {
            get { return Visibility.Visible; }
        }

        public ICommand LoadCommand
        {
            get { return new TargetCommand(DoLoadCommand); }
        }

        public ICommand RunCommand
        {
            get { return new TargetCommand(DoRunCommand); }
        }

        public ICommand StopCommand
        {
            get { return new TargetCommand(DoStopCommand); }
        }

        protected override void OnModelChanged()
        {
            DoModelChanged();
        }

        protected virtual void DoLoadCommand()
        {
            //Model.Projects.Clear();

            FolderBrowserDialog ofd = new FolderBrowserDialog();
#if DEBUG
            ofd.SelectedPath = @"C:\Sources\JaCoCoReader\JaCoCoReader.Core\Examples";
#else
            ofd.SelectedPath = RootPath;
#endif
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                RootPath = ofd.SelectedPath;
                TestProject testProject = PowerShellTestDiscoverer.GetTests(ofd.SelectedPath, null);

                TestSolution solution = new TestSolution();
                solution.Projects.Add(testProject);
                Model.Merge(solution);
                CleartestFilesByPath();
                OnModelChanged();
            }
        }

        protected void CleartestFilesByPath()
        {
            _testFilesByPath = null;
        }

        private string RootPath { get; set; }

        public virtual List<string> GetScriptFileNames()
        {
            if (string.IsNullOrEmpty(RootPath))
            {
                return new List<string>();
            }

            List<string> filenames = Directory.GetFiles(RootPath, "*.ps1", SearchOption.AllDirectories)
                .Where(filename => !filename.EndsWith(Constant.TestsPs1, StringComparison.InvariantCultureIgnoreCase)
                                    && !filename.EndsWith(Constant.StepsPs1, StringComparison.InvariantCultureIgnoreCase)).ToList();

            filenames.AddRange(Directory.GetFiles(RootPath, $"*{Constant.Psm1}", SearchOption.AllDirectories)
                .Where(filename => !filename.EndsWith(Constant.TestsPs1, StringComparison.InvariantCultureIgnoreCase)
                && !filename.EndsWith(Constant.StepsPs1, StringComparison.InvariantCultureIgnoreCase)));

            return filenames;
        }

        protected virtual void DoStopCommand()
        {
            //LoadModel(FileName);
            _executor?.Cancel();
        }


        protected virtual void DoRefreshCommand()
        {

        }

        private void DoRunCommand()
        {
            Task.Factory.StartNew(DoRunCommandAsync);

        }

        public bool Running
        {
            get { return _running; }
            set { SetProperty(ref _running, value); }
        }

        public string RunningTest
        {
            get { return _runningTest; }
            set { SetProperty(ref _runningTest, value); }
        }

        public ICommand OpenFileCommand
        {
            get { return new TargetCommand(DoOpenFileCommand); }
        }

        public bool ShowLines
        {
            get { return _showLines; }
            set { SetProperty(ref _showLines, value); }
        }

        public event TestModelChanged ModelChanged;

        private void DoModelChanged()
        {
            ThreadDispatcher.Dispatcher.Invoke(() => ModelChanged?.Invoke());
        }

        public event TestModelChanged ShowLinesModelChanged;

        protected virtual void DoOpenFileCommand()
        {
            
        }

        private void UpdateRunningTest(string name)
        {
            RunningTest = name;
        }

        private PowerShellTestExecutor _executor;
        private bool _clearCodeCoverage = true;
        private bool _showLines = true;

        private void DoRunCommandAsync()
        {
            DoRunCommandAsync(SelectedNode);
        }

        private void DoRunCommandAsync(TestModel node)
        {
            try
            {
                if (Running)
                {
                    return;
                }
                Running = true;

                RunContext content = new RunContext(UpdateRunningTest, _codeCoverage.SelectedCoveredScripts, GetScriptFileNames());
                _executor = new PowerShellTestExecutor();

                switch (node)
                {
                    case null:
                        _executor.RunTestSolution(Model, content);
                        break;
                    case TestSolution testSolution:
                        _executor.RunTestSolution(testSolution, content);
                        break;
                    case TestProject testProject:
                        _executor.RunTestProject(testProject, content);
                        break;
                    case TestFolder testFolder:
                        _executor.RunTestFolder(testFolder, content);
                        break;
                    case TestFile testFile:
                        _executor.RunTestFile(testFile, content);
                        break;
                    case TestDescribe testDescribe:
                        _executor.RunTestDescribe(testDescribe, content);
                        break;
                    case TestContext testContext:
                        _executor.RunTestDescribe(testContext.Parent, content);
                        break;
                    case TestIt testIt:
                        _executor.RunTestDescribe(testIt.Parent.Parent, content);
                        break;

                    case TestFeature testFeature:
                        _executor.RunTestFeature(testFeature, content);
                        break;
                    case TestScenario testScenario:
                        _executor.RunTestScenario(testScenario, content);
                        break;

                }

                Model.CalculateOutcome();

                _codeCoverage.Merge(content.Reports, ClearCodeCoverage);

                OnModelChanged();
            }
            finally
            {
                Running = false;
                RunningTest = "";
            }
        }

        public TestFile GetTestFileByPath(string path)
        {
            if (_testFilesByPath == null)
            {
                TestFilesByPath testFilesByPath = new TestFilesByPath();
                GetSourceFilePaths(Model, testFilesByPath);
                _testFilesByPath = testFilesByPath;
            }

            if (_testFilesByPath.TryGetValue(path, out TestFile testFile))
            {
                return testFile;
            }
            return null;
        }

        private void GetSourceFilePaths(TestModel model, TestFilesByPath testFilesByPath)
        {
            foreach (TestModel item in model.Items)
            {
                if (item is TestFile file)
                {
                    if (!testFilesByPath.ContainsKey(file.Path))
                    {
                        testFilesByPath.Add(file.Path, file);
                    }
                }
                else
                {
                    GetSourceFilePaths(item, testFilesByPath);
                }
            }
        }

        public void RunTests(string filePath, int lineNumber)
        {
            TestFile testFile = GetTestFileByPath(filePath);
            TestFile newFile = PowerShellTestDiscoverer.DiscoverTestFile(filePath);
            if (testFile == null)
            {
                testFile = newFile;
                AddTestFile(testFile);
            }
            else
            {
                testFile.Merge(newFile);
            }
            TestModel model = testFile.FindModelByLineNumber(lineNumber);
            if (model != null)
            {
                DoRunCommandAsync(model);
            }
        }

        private void AddTestFile(TestFile testFile)
        {
            TestProject project = Model.Projects.FirstOrDefault(p => p.Name == "new");
            if (project == null)
            {
                project = new TestProject
                {
                    Name = "new"
                };
                Model.Projects.Add(project);
            }
            project.Files.Add(testFile);
            project.NotifyItemsChanged();
        }
    }
}
