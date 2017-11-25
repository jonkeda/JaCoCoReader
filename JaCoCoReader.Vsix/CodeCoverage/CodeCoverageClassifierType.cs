using System.ComponentModel.Composition;
using JaCoCoReader.Core.ViewModels;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace JaCoCoReader.Vsix.CodeCoverage.Classifier
{
    /// <summary>
    /// Classification type definition export for CodeCoverageClassifier
    /// </summary>
    internal static class CodeCoverageClassifierType
    {

        // Miss
        public const string Miss = "CodeCoverageClassifierMiss";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Miss)]
        private static ClassificationTypeDefinition _missDefinition;

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = Miss)]
        [Name(Miss)]
        [UserVisible(true)] 
        [Order(Before = Priority.Default)] 
        internal sealed class CodeCoverageMissClassifierFormat : ClassificationFormatDefinition
        {
            public CodeCoverageMissClassifierFormat()
            {
                DisplayName = "PowerShell Code Coverage Miss";
                BackgroundColor = Colors.MissedBackground;
            }
        }

        // Hit
        public const string Hit = "CodeCoverageClassifierHit";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Hit)]
        private static ClassificationTypeDefinition _hitDefinition;

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = Hit)]
        [Name(Hit)]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
        internal sealed class CodeCoverageHitClassifierFormat : ClassificationFormatDefinition
        {
            public CodeCoverageHitClassifierFormat()
            {
                DisplayName = "PowerShell Code Coverage Hit";
                BackgroundColor = Colors.HitBackground;
            }
        }


        // None
        public const string None = "CodeCoverageClassifierNone";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(None)]
        private static ClassificationTypeDefinition _noneDefinition;

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = None)]
        [Name(None)]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
        internal sealed class CodeCoverageNoneClassifierFormat : ClassificationFormatDefinition
        {
            public CodeCoverageNoneClassifierFormat()
            {
                DisplayName = "PowerShell Code Coverage Default";
                BackgroundColor = Colors.DefaultBackground;
            }
        }


        // NotRun
        public const string NotRun = "CodeCoverageClassifierNotRun";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(NotRun)]
        private static ClassificationTypeDefinition _notRunDefinition;

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = NotRun)]
        [Name(NotRun)]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
        internal sealed class CodeCoverageNotRunClassifierFormat : ClassificationFormatDefinition
        {
            public CodeCoverageNotRunClassifierFormat()
            {
                DisplayName = "PowerShell Code Coverage Not run";
                BackgroundColor = Colors.NotRunBackground;
            }
        }
    }
}
