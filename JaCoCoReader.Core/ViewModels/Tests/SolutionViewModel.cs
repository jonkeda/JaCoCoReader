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
            ofd.SelectedPath = @"C:\Repos\Pester\Examples";
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
            if (SelectedNode is TestFile testFile)
            {
                PowerShellTestExecutor executor = new PowerShellTestExecutor();
                executor.RunTestFile(testFile, new IRunContext(), new IFrameworkHandle());
            }
        }
    }
}
