using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace JaCoCoReader.Vsix.Glyphs
{
    /// <summary>
    /// This class implements ITagger for TestTag.  It is responsible for creating
    /// TestTag TagSpans, which our GlyphFactory will then create glyphs for.
    /// </summary>
    internal class PesterTagger : ITagger<PesterTag>
    {
        private static readonly Regex FindText = new Regex(@"(\bdescribe\b|\bcontext\b|\bit\b)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// This method creates TestTag TagSpans over a set of SnapshotSpans.
        /// </summary>
        /// <param name="spans">A set of spans we want to get tags for.</param>
        /// <returns>The list of TestTag TagSpans.</returns>
        IEnumerable<ITagSpan<PesterTag>> ITagger<PesterTag>.GetTags(NormalizedSnapshotSpanCollection spans)
        {
            //Test: implement tagging
            foreach (SnapshotSpan curSpan in spans)
            {
                Match match = FindText.Match(curSpan.GetText());

                if (match.Success)
                {
                    SnapshotSpan testSpan = new SnapshotSpan(curSpan.Snapshot, new Span(curSpan.Start + match.Index, match.Length));
                    yield return new TagSpan<PesterTag>(testSpan, new PesterTag());
                }
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add {}
            remove {}
        }
    }
}
