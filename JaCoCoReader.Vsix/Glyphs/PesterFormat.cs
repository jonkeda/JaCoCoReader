using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace JaCoCoReader.Vsix.Glyphs
{
    /// <summary>
    /// Set the display values for the classification
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = PesterClassifierProvider.Name)]
    [Name(Name)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    internal sealed class PesterFormat : ClassificationFormatDefinition
    {
        public const string Name = "PesterFormat";

        public PesterFormat()
        {
            DisplayName = "Pester Tests";
            //BackgroundOpacity = 1;
            //BackgroundColor = Colors.Orange;
            ForegroundColor = Colors.Orange;

           // TextDecorations.Add(System.Windows.TextDecorations.Underline);
        }
    }
}