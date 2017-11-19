using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

namespace JaCoCoReader.Vsix.Tests
{
    [Export(typeof(ISuggestedActionsSourceProvider))]
    [Name("Run Test Suggested Actions")]
    [ContentType("Powershell")]
    internal class TestSuggestedActionsSourceProvider : ISuggestedActionsSourceProvider
    {
        [Import(typeof(ITextStructureNavigatorSelectorService))]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        [Import]
        IClassifierAggregatorService _classifierService;


        public ISuggestedActionsSource CreateSuggestedActionsSource(ITextView textView, ITextBuffer textBuffer)
        {
            if (textBuffer == null && textView == null)
            {
                return null;
            }
            IClassifier classifier = _classifierService.GetClassifier(textView.TextBuffer);
            return new TestSuggestedActionsSource(textView, textBuffer, classifier);
        }
    }
}
