using Microsoft.VisualStudio.Text;

namespace JaCoCoReader.Vsix.Extensions
{
    public static class TextBufferExtensions
    {
        public static ITextDocument GetTextDocument(this ITextBuffer textBuffer)
        {
            bool rc = textBuffer.Properties.TryGetProperty(typeof(ITextDocument), out ITextDocument textDoc);
            if (rc)
            {
                return textDoc;
            }
            return null;
        }
    }
}
