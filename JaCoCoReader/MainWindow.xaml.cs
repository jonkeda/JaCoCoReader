using JaCoCoReader.Core.ViewModels;

namespace JaCoCoReader
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new ReportViewModel();
        }
    }
}
