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
    public class SolutionViewModel : ModelViewModel<TestSolution>
    {
        private readonly ReportViewModel _report;

        public SolutionViewModel(ReportViewModel report)
        {
            _report = report;
        }

        private TestModel _selectedNode;
        private bool _running;

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

        protected virtual void DoLoadCommand()
        {
            FolderBrowserDialog ofd = new FolderBrowserDialog();
#if DEBUG
            ofd.SelectedPath = @"C:\Sources\JaCoCoReader\JaCoCoReader.Core\Examples";
#endif
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                TestProject testProject = PowerShellTestDiscoverer.GetTests(ofd.SelectedPath, null);
                Model.Projects.Add(testProject);
            }
        }

        protected virtual void DoRefreshCommand()
        {
            //LoadModel(FileName);
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

        private void DoRunCommandAsync()
        {
            try
            {
                if (Running)
                {
                    return;
                }
                Running = true;

                RunContext content = new RunContext();
                PowerShellTestExecutor executor = new PowerShellTestExecutor();

                switch (SelectedNode)
                {
                    case null:
                        executor.RunTestSolution(Model, content);
                        break;
                    case TestSolution testSolution:
                        executor.RunTestSolution(testSolution, content);
                        break;
                    case TestProject testProject:
                        executor.RunTestProject(testProject, content);
                        break;
                    case TestFolder testFolder:
                        executor.RunTestFolder(testFolder, content);
                        break;
                    case TestFile testFile:
                        executor.RunTestFile(testFile, content);
                        break;
                    case TestDescribe testDescribe:
                        executor.RunTestDescribe(testDescribe, content);
                        break;
                }
                _report.Merge(content.Reports);
            }
            finally
            {
                Running = false;
            }
        }
    }
}
