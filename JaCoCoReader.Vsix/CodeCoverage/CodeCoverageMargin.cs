using System;
using System.Windows;
using System.Windows.Controls;
using JaCoCoReader.Core.ViewModels.CodeCoverage;
using JaCoCoReader.Core.Views;
using JaCoCoReader.Vsix.Extensions;
using JaCoCoReader.Vsix.Services;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace JaCoCoReader.Vsix.CodeCoverage
{
    /// <summary>
    /// Margin's canvas and visual definition including both size and content
    /// </summary>
    internal class CodeCoverageMargin : Grid, IWpfTextViewMargin
    {
        private readonly IWpfTextView _textView;
        private readonly IVerticalScrollBar _scrollBar;

        /// <summary>
        /// Margin name.
        /// </summary>
        public const string MarginName = "PowershellMargin";

        /// <summary>
        /// A value indicating whether the object is disposed.
        /// </summary>
        private bool _isDisposed;

        private readonly CodeCoverageViewModel _codeCoverage;

        private readonly CodeCoverageMarginView _margin;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeCoverageMargin"/> class for a given <paramref name="textView"/>.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> to attach the margin to.</param>
        /// <param name="scrollBar"></param>
        public CodeCoverageMargin(IWpfTextView textView, IVerticalScrollBar scrollBar)
        {
            _textView = textView;
            _scrollBar = scrollBar;
            _scrollBar.TrackSpanChanged += ScrollBarOnTrackSpanChanged;
            Width = 10;

            // Add a green colored label that says "Hello CodeCoverageMargin"
            _margin = new CodeCoverageMarginView();
            UpdateScrollbarMargin();

            Children.Add(_margin);

            _codeCoverage = PowershellService.Current.CodeCoverage;
            SetCodeCoverageChanged();
            _codeCoverage.ModelChanged += SetCodeCoverageChanged;
            _codeCoverage.ShowHitsModelChanged += SetCodeCoverageChanged;

            if (!_codeCoverage.ShowLinesHit)
            {
                Visibility = Visibility.Collapsed;
            }
            else
            {
                Visibility = Visibility.Visible;
            }

        }

        private void ScrollBarOnTrackSpanChanged(object sender, EventArgs eventArgs)
        {
            UpdateScrollbarMargin();
        }

        private void UpdateScrollbarMargin()
        {
            _margin.Margin = new Thickness(0, _scrollBar.TrackSpanTop, 0, _scrollBar.ThumbHeight + (_textView.ViewportHeight - _scrollBar.TrackSpanBottom ));
        }

        private void SetCodeCoverageChanged()
        {
            ITextDocument textDocument = _textView.TextBuffer.GetTextDocument();
            if (textDocument == null)
            {
                return;
            }
            SourcefileViewModel sourceFile = _codeCoverage.GetSourceFileByPath(textDocument.FilePath);

            _margin.DataContext = sourceFile;
            if (sourceFile == null
                || !_codeCoverage.ShowLinesHit)
            {
                Visibility = Visibility.Collapsed;
            }
            else
            {
                Visibility = Visibility.Visible;
            }
        }

        #region IWpfTextViewMargin

        /// <summary>
        /// Gets the <see cref="FrameworkElement"/> that implements the visual representation of the margin.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The margin is disposed.</exception>
        public FrameworkElement VisualElement
        {
            // Since this margin implements Canvas, this is the object which renders
            // the margin.
            get
            {
                ThrowIfDisposed();
                return this;
            }
        }

        #endregion

        #region ITextViewMargin

        /// <summary>
        /// Gets the size of the margin.
        /// </summary>
        /// <remarks>
        /// For a horizontal margin this is the height of the margin,
        /// since the width will be determined by the <see cref="ITextView"/>.
        /// For a vertical margin this is the width of the margin,
        /// since the height will be determined by the <see cref="ITextView"/>.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">The margin is disposed.</exception>
        public double MarginSize
        {
            get
            {
                ThrowIfDisposed();

                // Since this is a horizontal margin, its width will be bound to the width of the text view.
                // Therefore, its size is its height.
                return ActualHeight;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the margin is enabled.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The margin is disposed.</exception>
        public bool Enabled
        {
            get
            {
                ThrowIfDisposed();
                _codeCoverage.ModelChanged -= SetCodeCoverageChanged;
                // The margin should always be enabled
                return true;
            }
        }

        /// <summary>
        /// Gets the <see cref="ITextViewMargin"/> with the given <paramref name="marginName"/> or null if no match is found
        /// </summary>
        /// <param name="marginName">The name of the <see cref="ITextViewMargin"/></param>
        /// <returns>The <see cref="ITextViewMargin"/> named <paramref name="marginName"/>, or null if no match is found.</returns>
        /// <remarks>
        /// A margin returns itself if it is passed its own name. If the name does not match and it is a container margin, it
        /// forwards the call to its children. Margin name comparisons are case-insensitive.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="marginName"/> is null.</exception>
        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return string.Equals(marginName, MarginName, StringComparison.OrdinalIgnoreCase) ? this : null;
        }

        /// <summary>
        /// Disposes an instance of <see cref="CodeCoverageMargin"/> class.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                GC.SuppressFinalize(this);
                _isDisposed = true;
            }
        }

        #endregion

        /// <summary>
        /// Checks and throws <see cref="ObjectDisposedException"/> if the object is disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(MarginName);
            }
        }
    }
}
