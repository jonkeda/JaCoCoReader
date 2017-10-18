using System.Windows;
using JaCoCoReader.Models;

namespace JaCoCoReader.UI.Controls
{
    public class CodeCoverageTextEditor : BindableTextEditor
    {
        public CodeCoverageTextEditor()
        {
            TextArea.TextView.BackgroundRenderers.Add(
                new CoverageBackgroundRenderer(this));
        }

        public LineHitDictionary LinesHit { get; set; }

        public static readonly DependencyProperty LinesHitProperty =
            DependencyProperty.Register(nameof(LinesHit), typeof(LineHitDictionary), typeof(CodeCoverageTextEditor), new PropertyMetadata(
                (obj, args) =>
                {
                    CodeCoverageTextEditor target = (CodeCoverageTextEditor)obj;
                    target.LinesHit = (LineHitDictionary)args.NewValue;
                }));
    }
}