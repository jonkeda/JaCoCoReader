using JaCoCoReader.Core.ViewModels.CodeCoverage;
using JaCoCoReader.Core.ViewModels.Tests;

namespace JaCoCoReader
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            CodeCoverage.DataContext = new ReportViewModel();
            Tests.DataContext = new SolutionViewModel();
        }
    }
}
