﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;
using JaCoCoReader.Core.Models.Tests;
using JaCoCoReader.Core.ViewModels.CodeCoverage;
using JaCoCoReader.Core.ViewModels.Tests;
using JaCoCoReader.Vsix.Extensions;
using JaCoCoReader.Vsix.Services;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="TestsAdornment"/> class.
        /// </summary>
        /// <param name="view">Text view to create the adornment for</param>
        /// <param name="classifier"></param>
        public TestsAdornment(IWpfTextView view, IClassifier classifier)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

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
            if (!_tests.ShowLines)
            {
                _layer.RemoveAllAdornments();
            }
            else
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
                if (!_tests.ShowLines)
                {
                    _layer.RemoveAllAdornments();
                }
                else
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
            //TestFile sourceFile = _tests.GetSourceFileByPath(textDocument.FilePath);
            //if (sourceFile == null)
            //{
            //    return;
            //}
            double viewportWidth = _view.ViewportWidth;
            foreach (ITextViewLine line in newOrReformattedLines)
            {
                int lineNumber = _view.TextSnapshot.GetLineNumberFromPosition(line.Extent.Start) + 1;

                IList<ClassificationSpan> classifiers = _classifier.GetClassificationSpans(line.Extent);

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

                //line.Snapshot;

                if (found)
                {
                    bool hit = true;

                    Rectangle rect = new Rectangle
                    {
                        Height = line.Height,
                        Width = viewportWidth,
                        Fill = hit ? Core.ViewModels.Colors.NotRunBackground : Core.ViewModels.Colors.MissedBackground,

                    };

                    Canvas.SetLeft(rect, _view.ViewportLeft);
                    Canvas.SetTop(rect, line.Top);
                    _layer.AddAdornment(line.Extent, null, rect);
                }
            }
        }
    }
}