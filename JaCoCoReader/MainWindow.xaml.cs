using JaCoCoReader.Core.ViewModels.CodeCoverage;
using JaCoCoReader.Core.ViewModels.Tests;

namespace JaCoCoReader
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            ReportViewModel reportViewModel = new ReportViewModel();
            CodeCoverage.DataContext = reportViewModel;
            Tests.DataContext = new SolutionViewModel(reportViewModel);
        }
    }
}
