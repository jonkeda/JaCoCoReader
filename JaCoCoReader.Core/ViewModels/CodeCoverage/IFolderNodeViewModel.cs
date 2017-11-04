using System.Windows.Media;
using JaCoCoReader.Core.UI.Controls;

namespace JaCoCoReader.Core.ViewModels.CodeCoverage
{
    public interface IFolderNodeViewModel : ITreeViewItem
    {
        string Description { get; }

        int MissedLines { get; }

        double MissedLinesPercentage { get; }

        int CoveredLines { get; }

        double CoveredLinesPercentage { get; }

        int TotalLines { get; }

        Brush Background { get; }
    }
}