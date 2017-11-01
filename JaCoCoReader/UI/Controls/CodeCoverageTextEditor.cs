using System.Windows;
using JaCoCoReader.Core.Models;

namespace JaCoCoReader.UI.Controls
{
    public class CodeCoverageTextEditor : BindableTextEditor
    {
        public CodeCoverageTextEditor()
        {
            ShowLinesHit = true;
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

        public static readonly DependencyProperty ShowLinesHitProperty = DependencyProperty.Register(
            nameof(ShowLinesHit), typeof(bool), typeof(CodeCoverageTextEditor), new PropertyMetadata(true,
                (obj, args) =>
                {
                    CodeCoverageTextEditor target = (CodeCoverageTextEditor)obj;
                    target.ShowLinesHit = (bool)args.NewValue;
                }));

        private bool _showLinesHit;

        public bool ShowLinesHit
        {
            get { return _showLinesHit; }
            set
            {
                _showLinesHit = value;
                UpdateLayout();
            }
        }
    }
}