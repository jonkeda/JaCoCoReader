using System.ComponentModel.Composition;
using JaCoCoReader.Vsix.FileExtension;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace JaCoCoReader.Vsix.Glyphs
{
    /// <summary>
    /// Export a <see cref="IClassifierProvider"/>
    /// </summary>
    [Export(typeof(IClassifierProvider))]
    [ContentType(FileAndContentTypeDefinitions.ContentType)]
    internal class PesterClassifierProvider : IClassifierProvider
    {
        public const string Name = "pesterCtd";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Name)]
        internal ClassificationTypeDefinition TestClassificationType;

        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry;

        [Import]
        internal IBufferTagAggregatorFactoryService TagAggregatorFactory;

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            IClassificationType classificationType = ClassificationRegistry.GetClassificationType(Name);

            ITagAggregator<PesterTag> tagAggregator = TagAggregatorFactory.CreateTagAggregator<PesterTag>(buffer);
            return new PesterClassifier(tagAggregator, classificationType);
        }
    }
}
