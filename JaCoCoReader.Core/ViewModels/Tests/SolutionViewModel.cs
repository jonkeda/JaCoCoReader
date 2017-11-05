using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using JaCoCoReader.Core.Models.Tests;
using JaCoCoReader.Core.Services;
using JaCoCoReader.Core.UI;

namespace JaCoCoReader.Core.ViewModels.Tests
{
    public class SolutionViewModel : ModelViewModel<TestSolution>
    {
        private TestModel _selectedNode;

        public TestModel SelectedNode
        {
            get { return _selectedNode; }
            set { SetProperty(ref _selectedNode, value); }
        }

        public ICommand RefreshCommand
        {
            get { return new TargetCommand(DoRefreshCommand); }
        }

        public ICommand LoadCommand
        {
            get { return new TargetCommand(DoLoadCommand); }
        }

        public ICommand RunCommand
        {
            get { return new TargetCommand(DoRunCommand); }
        }

        private void DoLoadCommand()
        {
            FolderBrowserDialog ofd = new FolderBrowserDialog();
            ofd.SelectedPath = @"C:\Sources\JaCoCoReader\JaCoCoReader.Core\Examples";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                TestProject testProject = PowerShellTestDiscoverer.GetTests(ofd.SelectedPath, null);
                Model.Projects.Add(testProject);
            }
        }

        private void DoRefreshCommand()
        {
            //LoadModel(FileName);
        }

        private void DoRunCommand()
        {
            Task.Factory.StartNew(DoRunCommandAsync);

        }

        private void DoRunCommandAsync()
        {
            RunContext content = new RunContext();
            PowerShellTestExecutor executor = new PowerShellTestExecutor();

            switch (SelectedNode)
            {
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
        }
    }
}
