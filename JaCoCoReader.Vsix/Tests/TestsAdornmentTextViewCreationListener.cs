using System.ComponentModel.Composition;
using JaCoCoReader.Core.Constants;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace JaCoCoReader.Vsix.Tests
{
    /// <summary>
    /// Establishes an <see cref="IAdornmentLayer"/> to place the adornment on and exports the <see cref="IWpfTextViewCreationListener"/>
    /// that instantiates the adornment on the event of a <see cref="IWpfTextView"/>'s creation
    /// </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType(Constant.PowerShell)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class TestsAdornmentTextViewCreationListener : IWpfTextViewCreationListener
    {
        // Disable "Field is never assigned to..." and "Field is never used" compiler's warnings. Justification: the field is used by MEF.
#pragma warning disable 649, 169

        [Import]
        IClassifierAggregatorService _classifierService;


        /// <summary>
        /// Defines the adornment layer for the adornment. This layer is ordered
        /// after the selection layer in the Z-order
        /// </summary>
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("TestsAdornment")]
        [Order(Before = PredefinedAdornmentLayers.Selection)]
        private AdornmentLayerDefinition editorAdornmentLayer;

#pragma warning restore 649, 169

        #region IWpfTextViewCreationListener

        /// <summary>
        /// Called when a text view having matching roles is created over a text data model having a matching content type.
        /// Instantiates a CodeCoverageAdornment manager when the textView is created.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> upon which the adornment should be placed</param>
        public void TextViewCreated(IWpfTextView textView)
        {
            IClassifier classifier = _classifierService.GetClassifier(textView.TextBuffer);

            // The adornment will listen to any event that changes the layout (text changes, scrolling, etc)
            new TestsAdornment(textView, classifier);
        }

        #endregion
    }
}
