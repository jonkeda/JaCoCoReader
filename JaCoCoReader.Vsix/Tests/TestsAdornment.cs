﻿using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using JaCoCoReader.Core.Models.Tests;
using JaCoCoReader.Core.Services;
using JaCoCoReader.Core.ViewModels.Tests;
using JaCoCoReader.Vsix.CodeCoverage.Classifier;
using JaCoCoReader.Vsix.Extensions;
using JaCoCoReader.Vsix.Services;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace JaCoCoReader.Vsix.Tests
{
    /// <summary>
    /// CodeCoverageAdornment places red boxes behind all the "a"s in the editor window
    /// </summary>
    internal sealed class TestsAdornment
    {
        /// <summary>
        /// The layer of the adornment.
        /// </summary>
        private readonly IAdornmentLayer _layer;

        /// <summary>
        /// Text view where the adornment is created.
        /// </summary>
        private readonly IWpfTextView _view;

        private readonly IClassifier _classifier;

        private readonly TestsViewModel _tests;

        public Brush HitBackground;
        public Brush MissedBackground;
        public Brush NotRunBackground;


        /// <summary>
        /// Initializes a new instance of the <see cref="TestsAdornment"/> class.
        /// </summary>
        /// <param name="view">Text view to create the adornment for</param>
        /// <param name="classifier"></param>
        /// <param name="classificationRegistry"></param>
        /// <param name="formatMap"></param>
        public TestsAdornment(IWpfTextView view, IClassifier classifier,
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

            IClassificationType notRunClassificationType = classificationRegistry.GetClassificationType(CodeCoverageClassifierType.NotRun);
            IClassificationFormatMap notRunFormat = formatMap.GetClassificationFormatMap(view);
            TextFormattingRunProperties notRunText = notRunFormat.GetTextProperties(notRunClassificationType);
            NotRunBackground = notRunText.BackgroundBrush;


            _layer = view.GetAdornmentLayer("TestsAdornment");

            _view = view;
            _classifier = classifier;
            _view.LayoutChanged += OnLayoutChanged;

            _tests = PowershellService.Current.Tests;
            _tests.ModelChanged += ShowHitsModelChanged;
            _tests.ShowLinesModelChanged += ShowHitsModelChanged;
        }

        private void ShowHitsModelChanged()
        {
            _layer.RemoveAllAdornments();
            if (_tests.ShowLines)
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
                if (_tests.ShowLines)
                {
                    UpdateLines(_view.TextViewLines);
                }
            }
            else
            {
                if (!_tests.ShowLines)
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
            TestFile sourceFile = _tests.GetTestFileByPath(textDocument.FilePath);

            double viewportWidth = _view.ViewportWidth;
            foreach (ITextViewLine line in newOrReformattedLines)
            {
                int lineNumber;
                bool found = line.Extent.FindTest(_view, _classifier, out lineNumber);
                if (found)
                {
                    TestOutcome outcome = TestOutcome.None;
                    if (sourceFile != null)
                    {
                        outcome = sourceFile.GetOutcome(lineNumber);
                    }
                    Brush brush;
                    switch (outcome)
                    {
                        case TestOutcome.Failed:
                            brush = MissedBackground;
                            break;
                        case TestOutcome.Passed:
                            brush = HitBackground;
                            break;
                        case TestOutcome.None:
                        case TestOutcome.Skipped:
                        case TestOutcome.NotFound:
                        default:
                            brush = NotRunBackground;
                            break;
                    }

                    Rectangle rect = new Rectangle
                    {
                        Height = line.Height,
                        Width = viewportWidth,
                        Fill = brush,
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
