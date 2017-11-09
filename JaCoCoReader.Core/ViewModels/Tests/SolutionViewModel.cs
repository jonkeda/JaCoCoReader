using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using JaCoCoReader.Core.Models.Tests;
using JaCoCoReader.Core.Services;
using JaCoCoReader.Core.UI;
using JaCoCoReader.Core.ViewModels.CodeCoverage;

namespace JaCoCoReader.Core.ViewModels.Tests
{
    public class TestsViewModel : ModelViewModel<TestSolution>
    {
        private readonly CodeCoverageViewModel _report;

        public TestsViewModel(CodeCoverageViewModel report)
        {
            _report = report;
        }

        private TestModel _selectedNode;
        private bool _running;
        private string _runningTest;

        public TestModel SelectedNode
        {
            get { return _selectedNode; }
            set { SetProperty(ref _selectedNode, value); }
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

        protected virtual void DoLoadCommand()
        {
            Model.Projects.Clear();

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
                Model.Projects.Add(testProject);
            }
        }

        private string RootPath { get; set; }

        public virtual List<string> GetScriptFileNames()
        {
            if (string.IsNullOrEmpty(RootPath))
            {
                return new List<string>();
            }

            List<string> filenames = Directory.GetFiles(RootPath, "*.ps1", SearchOption.AllDirectories).Where(filename => !filename.EndsWith(".tests.ps1", StringComparison.InvariantCultureIgnoreCase)).ToList();

            filenames.AddRange(Directory.GetFiles(RootPath, "*.psm1", SearchOption.AllDirectories).Where(filename => !filename.EndsWith(".tests.psm1", StringComparison.InvariantCultureIgnoreCase)));

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

        protected virtual void DoOpenFileCommand()
        {
            
        }

        private void UpdateRunningTest(string name)
        {
            RunningTest = name;
        }

        private PowerShellTestExecutor _executor;

        private void DoRunCommandAsync()
        {
            try
            {
                if (Running)
                {
                    return;
                }
                Running = true;

                RunContext content = new RunContext(UpdateRunningTest, _report.SelectedCoveredScripts, GetScriptFileNames());
                _executor = new PowerShellTestExecutor();

                switch (SelectedNode)
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
                }
                _report.Merge(content.Reports);
            }
            finally
            {
                Running = false;
                RunningTest = "";
            }
        }
    }
}
