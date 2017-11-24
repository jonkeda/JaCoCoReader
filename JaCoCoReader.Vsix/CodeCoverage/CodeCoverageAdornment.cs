using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using JaCoCoReader.Core.ViewModels.CodeCoverage;
using JaCoCoReader.Vsix.CodeCoverage.Classifier;
using JaCoCoReader.Vsix.Extensions;
using JaCoCoReader.Vsix.Services;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
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

        private readonly CodeCoverageViewModel _codeCoverage;

        public Brush HitBackground;
        public Brush MissedBackground;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeCoverageAdornment"/> class.
        /// </summary>
        /// <param name="view">Text view to create the adornment for</param>
        /// <param name="classificationRegistry"></param>
        /// <param name="formatMap"></param>
        public CodeCoverageAdornment(IWpfTextView view, 
            IClassificationTypeRegistryService classificationRegistry,
            IClassificationFormatMapService formatMap)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            IClassificationType missClassificationType = classificationRegistry.GetClassificationType(CodeCoverageClassifierType.Miss);
            IClassificationFormatMap missFormat = formatMap.GetClassificationFormatMap(view);
            TextFormattingRunProperties missText = missFormat.GetTextProperties(missClassificationType);
            MissedBackground = missText.BackgroundBrush;

            IClassificationType hitClassificationType = classificationRegistry.GetClassificationType(CodeCoverageClassifierType.Hit);
            IClassificationFormatMap hitFormat = formatMap.GetClassificationFormatMap(view);
            TextFormattingRunProperties hitText = hitFormat.GetTextProperties(hitClassificationType);
            HitBackground = hitText.BackgroundBrush;


            _layer = view.GetAdornmentLayer("CodeCoverageAdornment");

            _view = view;
            _view.LayoutChanged += OnLayoutChanged;

            _codeCoverage = PowershellService.Current.CodeCoverage;
            _codeCoverage.ModelChanged += ShowHitsModelChanged;
            _codeCoverage.ShowHitsModelChanged += ShowHitsModelChanged;
        }

        private void ShowHitsModelChanged()
        {
            _layer.RemoveAllAdornments();
            if (_codeCoverage.ShowLinesHit)
            {
                UpdateLines(_view.TextViewLines);
            }
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
                if (_codeCoverage.ShowLinesHit)
                {
                    UpdateLines(_view.TextViewLines);
                }
            }
            else
            {
                if (!_codeCoverage.ShowLinesHit)
                {
                    _layer.RemoveAllAdornments();
                }
                else
                {
                    UpdateLines(e.NewOrReformattedLines);
                }
            }
        }

        private void UpdateLines(IEnumerable<ITextViewLine> newOrReformattedLines)
        {
            ITextDocument textDocument = _view.TextBuffer.GetTextDocument();
            if (textDocument == null)
            {
                return;
            }
            SourcefileViewModel sourceFile = PowershellService.Current.CodeCoverage.GetSourceFileByPath(textDocument.FilePath);
            if (sourceFile == null)
            {
                return;
            }
            double viewportWidth = _view.ViewportWidth;
            foreach (ITextViewLine line in newOrReformattedLines)
            {
                int lineNumber = _view.TextSnapshot.GetLineNumberFromPosition(line.Extent.Start) + 1;

                if (sourceFile.LinesHit.ContainsKey(lineNumber))
                {
                    bool hit = sourceFile.LinesHit[lineNumber];

                    Rectangle rect = new Rectangle
                    {
                        Height = line.Height,
                        Width = viewportWidth,
                        Fill = hit ? HitBackground : MissedBackground,
                        Opacity = 0.35
                    };

                    Canvas.SetLeft(rect, _view.ViewportLeft);
                    Canvas.SetTop(rect, line.Top);
                    _layer.AddAdornment(line.Extent, null, rect);
                }
            }
        }
    }
}
