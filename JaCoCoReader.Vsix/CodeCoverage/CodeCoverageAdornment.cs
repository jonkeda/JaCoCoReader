using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using JaCoCoReader.Core.ViewModels.CodeCoverage;
using JaCoCoReader.Vsix.Extensions;
using JaCoCoReader.Vsix.Services;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Pen = System.Windows.Media.Pen;

namespace JaCoCoReader.Vsix
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
        /// Adornment brush.
        /// </summary>
        private readonly Brush _brush;

        /// <summary>
        /// Adornment pen.
        /// </summary>
        private readonly Pen _pen;

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

            // Create the pen and brush to color the box behind the a's
            _brush = new SolidColorBrush(Color.FromArgb(0x20, 0x00, 0x00, 0xff));
            _brush.Freeze();

            var penBrush = new SolidColorBrush(Colors.Red);
            penBrush.Freeze();
            _pen = new Pen(penBrush, 0.5);
            _pen.Freeze();
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

            IWpfTextView textView = (IWpfTextView)sender;
            ITextDocument textDocument = textView.TextBuffer.GetTextDocument();
            SourcefileViewModel sourceFile = CodeCoverageService.Current.Report.GetSourceFileByPath(textDocument.FilePath);
            if (sourceFile == null)
            {
                return;
            }
            double viewportWidth = textView.ViewportWidth;
            foreach (ITextViewLine line in e.NewOrReformattedLines)
            {
                int lineNumber = textView.TextSnapshot.GetLineNumberFromPosition(line.Extent.Start) + 1;

                if (sourceFile.LinesHit.ContainsKey(lineNumber))
                {
                    bool hit = sourceFile.LinesHit[lineNumber];

                    Rectangle rect = new Rectangle
                    {
                        Height = line.Height,
                        Width = viewportWidth,
                        Fill = hit ? JaCoCoReader.Core.ViewModels.Colors.HitBackground : JaCoCoReader.Core.ViewModels.Colors.MissedBackground
                    };

                    Canvas.SetLeft(rect, textView.ViewportLeft);
                    Canvas.SetTop(rect, line.Top);
                    _layer.AddAdornment(line.Extent, null, rect);
                }
            }
        }

    }
}
