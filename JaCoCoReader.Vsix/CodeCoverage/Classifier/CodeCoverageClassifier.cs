using System;
using System.Collections.Generic;
using JaCoCoReader.Core.ViewModels.CodeCoverage;
using JaCoCoReader.Vsix.Extensions;
using JaCoCoReader.Vsix.Services;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;

namespace JaCoCoReader.Vsix.CodeCoverage.Classifier
{
    /// <summary>
    /// Classifier that classifies all text as an instance of the "CodeCoverageClassifier" classification type.
    /// </summary>
    internal class CodeCoverageClassifier : IClassifier
    {
        public const string MissName = "CodeCoverageClassifierMiss";
        public const string HitName = "CodeCoverageClassifierHit";


        /// <summary>
        /// Classification type.
        /// </summary>
        private readonly IClassificationType _missClassificationType;
        private readonly IClassificationType _hitClassificationType;

        private readonly ITextBuffer _buffer;
        private readonly SourcefileViewModel _sourceFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeCoverageClassifier"/> class.
        /// </summary>
        
        /// <param name="buffer"></param>
        /// <param name="registry">Classification registry.</param>
        internal CodeCoverageClassifier(ITextBuffer buffer, IClassificationTypeRegistryService registry)
        {
            _missClassificationType = registry.GetClassificationType(CodeCoverageClassifierType.Miss);
            _hitClassificationType = registry.GetClassificationType(CodeCoverageClassifierType.Hit);
            _buffer = buffer;
            ITextDocument textDocument = _buffer.GetTextDocument();
            if (textDocument == null)
            {
                return;
            }
            _sourceFile = PowershellService.Current.CodeCoverage.GetSourceFileByPath(textDocument.FilePath);
        }

        #region IClassifier

#pragma warning disable 67

        /// <summary>
        /// An event that occurs when the classification of a span of text has changed.
        /// </summary>
        /// <remarks>
        /// This event gets raised if a non-text change would affect the classification in some way,
        /// for example typing /* would cause the classification to change in C# without directly
        /// affecting the span.
        /// </remarks>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

#pragma warning restore 67

        /// <summary>
        /// Gets all the <see cref="ClassificationSpan"/> objects that intersect with the given range of text.
        /// </summary>
        /// <remarks>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </remarks>
        /// <param name="span">The span currently being classified.</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification.</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            List<ClassificationSpan> result = new List<ClassificationSpan>();
            //if (_sourceFile != null)
            //{
                //int lineNumber = _textView.TextSnapshot.GetLineNumberFromPosition(span.Start) + 1;

                //if (_sourceFile.LinesHit.ContainsKey(lineNumber))
                //{
                    IClassificationType type;
                    //if (_sourceFile.LinesHit[lineNumber])
                    //{
                        type = _hitClassificationType;
                    //}
                    //else
                    //{
                    //    type = _missClassificationType;
                    //}

                    //result.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start, span.Length)), type));
                //}
            //}
            return result;
        }

        #endregion
    }
}
