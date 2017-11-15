using System;
using System.ComponentModel.Composition;
using JaCoCoReader.Vsix.FileExtension;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace JaCoCoReader.Vsix.Glyphs
{
    /// <summary>
    /// Export a <see cref="ITaggerProvider"/>
    /// </summary>
    [Export(typeof(ITaggerProvider))]
    [ContentType(FileAndContentTypeDefinitions.ContentType)]
    [TagType(typeof(PesterTag))]
    internal class PesterTaggerProvider : ITaggerProvider
    {
        /// <summary>
        /// Creates an instance of our custom TestTagger for a given buffer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer">The buffer we are creating the tagger for.</param>
        /// <returns>An instance of our custom TestTagger.</returns>
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            return new PesterTagger() as ITagger<T>;
        }
    }
}
