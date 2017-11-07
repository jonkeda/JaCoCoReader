using JaCoCoReader.Vsix.Services;

namespace JaCoCoReader.Vsix.Tests
{


    /// <summary>
    /// Interaction logic for TestsWindowControl.
    /// </summary>
    public partial class TestsWindowControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestsWindowControl"/> class.
        /// </summary>
        public TestsWindowControl()
        {
            this.InitializeComponent();

            DataContext = CodeCoverageService.Current.Solution;
        }


    }
}