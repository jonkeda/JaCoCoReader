using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;

namespace JaCoCoReader.Vsix.Tests
{
    public static class TestsHelpers
    { 
        public static bool FindTest(this SnapshotSpan extent, ITextView view, IClassifier classifier, out int lineNumber)
        {
            lineNumber = view.TextSnapshot.GetLineNumberFromPosition(extent.Start) + 1;

            IList<ClassificationSpan> classifiers = classifier.GetClassificationSpans(extent);

            ClassificationSpan cspan = classifiers.FirstOrDefault(c =>
                c.ClassificationType.Classification.Contains("PowerShellCommand"));

            bool found = false;
            string text = cspan?.Span.GetText();
            if (string.Equals(text, "describe", StringComparison.InvariantCultureIgnoreCase))
            {
                found = true;
            }
            else if (string.Equals(text, "context", StringComparison.InvariantCultureIgnoreCase))
            {
                found = true;
            }
            else if (string.Equals(text, "it", StringComparison.InvariantCultureIgnoreCase))
            {
                found = true;
            }
            return found;
        }
    }
}