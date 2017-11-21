using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace JaCoCoReader.Vsix.CodeCoverage.Classifier
{
    /// <summary>
    /// Defines an editor format for the CodeCoverageClassifier type that has a purple background
    /// and is underlined.
    /// </summary>
    //[Export(typeof(EditorFormatDefinition))]
    //[ClassificationType(ClassificationTypeNames = CodeCoverageClassifierType.Miss)]
    //[Name(CodeCoverageClassifier.HitName)]
    //[UserVisible(true)] // This should be visible to the end user
    //[Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    //internal sealed class CodeCoverageClassifierFormat : ClassificationFormatDefinition
    //{
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="CodeCoverageClassifierFormat"/> class.
    //    /// </summary>
    //    public CodeCoverageClassifierFormat()
    //    {
    //        this.DisplayName = "Powershell Code Coverage Hit"; // Human readable version of the name
    //        this.BackgroundColor = Colors.BlueViolet;
    //        this.TextDecorations = System.Windows.TextDecorations.Underline;
    //    }
    //}
}
