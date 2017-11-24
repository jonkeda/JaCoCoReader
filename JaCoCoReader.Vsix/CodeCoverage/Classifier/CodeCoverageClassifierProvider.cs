using System.ComponentModel.Composition;
using JaCoCoReader.Core.Constants;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace JaCoCoReader.Vsix.CodeCoverage.Classifier
{
    /// <summary>
    /// Classifier provider. It adds the classifier to the set of classifiers.
    /// </summary>
    [Export(typeof(IClassifierProvider))]
    [ContentType(Constant.PowerShell)] // This classifier applies to all text files.
    internal class CodeCoverageClassifierProvider : IClassifierProvider
    {
#pragma warning disable 649

        /// <summary>
        /// Classification registry to be used for getting a reference
        /// to the custom classification type later.
        /// </summary>
        [Import]
        private IClassificationTypeRegistryService _classificationRegistry;

#pragma warning restore 649

        #region IClassifierProvider

        /// <summary>
        /// Gets a classifier for the given text buffer.
        /// </summary>
        /// <param name="buffer">The <see cref="ITextBuffer"/> to classify.</param>
        /// <returns>A classifier for the text buffer, or null if the provider cannot do so in its current state.</returns>
        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() => new CodeCoverageClassifier(buffer, _classificationRegistry));
        }

        #endregion
    }
}
