using JaCoCoReader.Vsix.Services;

namespace JaCoCoReader.Vsix
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for CodeCoverageWindowControl.
    /// </summary>
    public partial class CodeCoverageWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeCoverageWindowControl"/> class.
        /// </summary>
        public CodeCoverageWindowControl()
        {
            InitializeComponent();

            DataContext = CodeCoverageService.Current.Report;
        }
    }
}