using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace JaCoCoReader.Vsix.Glyphs
{
    /// <summary>
    /// Test tag classifier finds every instance of TestTag within a given span.
    /// </summary>
    internal class PesterClassifier : IClassifier
    {
        private readonly IClassificationType _classificationType;
        private readonly ITagAggregator<PesterTag> _tagger;

        internal PesterClassifier(ITagAggregator<PesterTag> tagger, IClassificationType testType)
        {
            _tagger = tagger;
            _classificationType = testType;
        }

        /// <summary>
        /// Get every TestTag instance within the given span. Generally, the span in 
        /// question is the displayed portion of the file currently open in the Editor
        /// </summary>
        /// <param name="span">The span of text that will be searched for Test tags</param>
        /// <returns>A list of every relevant tag in the given span</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            IList<ClassificationSpan> classifiedSpans = new List<ClassificationSpan>();

            IEnumerable<IMappingTagSpan<PesterTag>> tags = _tagger.GetTags(span);

            foreach (IMappingTagSpan<PesterTag> tagSpan in tags)
            {
                SnapshotSpan testSpan = tagSpan.Span.GetSpans(span.Snapshot).First();
                classifiedSpans.Add(new ClassificationSpan(testSpan, _classificationType));
            }

            return classifiedSpans;
        }

        /// <summary>
        /// Create an event for when the Classification changes
        /// </summary>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged
        {
            add {}
            remove {}
        }
    }
}
