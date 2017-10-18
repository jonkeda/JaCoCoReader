using System.Windows.Media;
using JaCoCoReader.UI.Controls;

namespace JaCoCoReader.ViewModels
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