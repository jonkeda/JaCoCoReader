using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JaCoCoReader.Core.Models.Tests;
using JaCoCoReader.Core.ViewModels.Tests;
using JaCoCoReader.Vsix.Extensions;
using JaCoCoReader.Vsix.Services;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;

namespace JaCoCoReader.Vsix.Tests
{
    internal class TestSuggestedActionsSource : ISuggestedActionsSource
    {
        private readonly ITextBuffer _textBuffer;
        private readonly ITextView _view;
        private readonly IClassifier _classifier;

        private readonly TestsViewModel _tests;

        public TestSuggestedActionsSource(ITextView textView, ITextBuffer textBuffer, IClassifier classifier)
        {
            _classifier = classifier;
            _tests = PowershellService.Current.Tests;

            _textBuffer = textBuffer;
            _view = textView;
        }

#pragma warning disable 0067
        public event EventHandler<EventArgs> SuggestedActionsChanged;
#pragma warning restore 0067

        public void Dispose()
        {
        }

        public IEnumerable<SuggestedActionSet> GetSuggestedActions(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            if (IsTest(range, out string testFile, out int lineNumber))
            {
                RunTestSuggestedAction runTestAction = new RunTestSuggestedAction(_tests, testFile, lineNumber);
                return new[] { new SuggestedActionSet(new ISuggestedAction[] { runTestAction }) };
            }
            return Enumerable.Empty<SuggestedActionSet>();
        }

        public Task<bool> HasSuggestedActionsAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => IsTest(range, out string _, out int _));
        }

        private bool IsTest(SnapshotSpan range, out string testFile, out int lineNumber)
        {
            ITextDocument textDocument = _textBuffer.GetTextDocument();
            if (textDocument == null)
            {
                testFile = null;
                lineNumber = -1;
                return false;
            }
            testFile = textDocument.FilePath;

            return range.FindTest(_view, _classifier, out lineNumber);
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            telemetryId = Guid.Empty;
            return false;
        }
    }
}
