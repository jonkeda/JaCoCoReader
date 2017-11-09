using System;
using System.Windows.Controls;
using System.Windows.Shapes;
using JaCoCoReader.Core.ViewModels.CodeCoverage;
using JaCoCoReader.Vsix.Extensions;
using JaCoCoReader.Vsix.Services;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace JaCoCoReader.Vsix.CodeCoverage
{
    /// <summary>
    /// CodeCoverageAdornment places red boxes behind all the "a"s in the editor window
    /// </summary>
    internal sealed class CodeCoverageAdornment
    {
        /// <summary>
        /// The layer of the adornment.
        /// </summary>
        private readonly IAdornmentLayer _layer;

        /// <summary>
        /// Text view where the adornment is created.
        /// </summary>
        private readonly IWpfTextView _view;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeCoverageAdornment"/> class.
        /// </summary>
        /// <param name="view">Text view to create the adornment for</param>
        public CodeCoverageAdornment(IWpfTextView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            _layer = view.GetAdornmentLayer("CodeCoverageAdornment");

            _view = view;
            _view.LayoutChanged += OnLayoutChanged;
        }

        /// <summary>
        /// Handles whenever the text displayed in the view changes by adding the adornment to any reformatted lines
        /// </summary>
        /// <remarks><para>This event is raised whenever the rendered text displayed in the <see cref="ITextView"/> changes.</para>
        /// <para>It is raised whenever the view does a layout (which happens when DisplayTextLineContainingBufferPosition is called or in response to text or classification changes).</para>
        /// <para>It is also raised whenever the view scrolls horizontally or when its size changes.</para>
        /// </remarks>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        internal void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (e.OldSnapshot != e.NewSnapshot 
                && e.OldSnapshot.Version.Changes.IncludesLineChanges)
            {
                _layer.RemoveAllAdornments();
            }

           
            ITextDocument textDocument = _view.TextBuffer.GetTextDocument();
            SourcefileViewModel sourceFile = PowershellService.Current.CodeCoverage.GetSourceFileByPath(textDocument.FilePath);
            if (sourceFile == null)
            {
                return;
            }
            double viewportWidth = _view.ViewportWidth;
            foreach (ITextViewLine line in e.NewOrReformattedLines)
            {
                int lineNumber = _view.TextSnapshot.GetLineNumberFromPosition(line.Extent.Start) + 1;

                if (sourceFile.LinesHit.ContainsKey(lineNumber))
                {
                    bool hit = sourceFile.LinesHit[lineNumber];

                    Rectangle rect = new Rectangle
                    {
                        Height = line.Height,
                        Width = viewportWidth,
                        Fill = hit ? Core.ViewModels.Colors.HitBackground : Core.ViewModels.Colors.MissedBackground
                    };

                    Canvas.SetLeft(rect, _view.ViewportLeft);
                    Canvas.SetTop(rect, line.Top);
                    _layer.AddAdornment(line.Extent, null, rect);
                }
            }
        }

    }
}
