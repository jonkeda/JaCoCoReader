using JaCoCoReader.Core.ViewModels.CodeCoverage;
using JaCoCoReader.Core.ViewModels.Tests;

namespace JaCoCoReader
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            CodeCoverageViewModel reportViewModel = new CodeCoverageViewModel();
            CodeCoverage.DataContext = reportViewModel;
            Tests.DataContext = new TestsViewModel(reportViewModel);
        }
    }
}
