using System.Windows;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace JaCoCoReader.Vsix.Glyphs
{
    /// <summary>
    /// This class implements IGlyphFactory, which provides the visual
    /// element that will appear in the glyph margin.
    /// </summary>
    internal class PesterGlyphFactory : IGlyphFactory
    {

        public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag)
        {
            return new PesterGlyph();
        }

    }
}
