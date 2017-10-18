using System.Linq;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;

namespace JaCoCoReader.UI.Controls
{
    public class CoverageBackgroundRenderer : IBackgroundRenderer
    {
        static readonly Pen Pen;

        public static readonly SolidColorBrush MissedBackground;
        public static readonly SolidColorBrush HitBackground;
        public static readonly SolidColorBrush DefaultBackground;

        readonly CodeCoverageTextEditor editor;

        static CoverageBackgroundRenderer()
        {
            MissedBackground = new SolidColorBrush(Color.FromRgb(0xff, 0xdd, 0xdd)); MissedBackground.Freeze();
            HitBackground = new SolidColorBrush(Color.FromRgb(0xdd, 0xff, 0xdd)); HitBackground.Freeze();
            DefaultBackground = Brushes.Transparent;

            SolidColorBrush blackBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)); blackBrush.Freeze();
            Pen = new Pen(blackBrush, 0.0);
        }

        public CoverageBackgroundRenderer(CodeCoverageTextEditor editor)
        {
            this.editor = editor;
        }

        public KnownLayer Layer
        {
            get { return KnownLayer.Background; }
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (editor?.LinesHit == null)
            {
                return;
            }

            foreach (var v in textView.VisualLines)
            {
                var rc = BackgroundGeometryBuilder.GetRectsFromVisualSegment(textView, v, 0, 1000).First();

                int linenum = v.FirstDocumentLine.LineNumber;

                bool hit;
                if (editor.LinesHit.TryGetValue(linenum, out hit))
                {
                    Brush brush;
                    if (hit)
                    {
                        brush = HitBackground;
                    }
                    else
                    {
                        brush = MissedBackground;
                    }

                    drawingContext.DrawRectangle(brush, Pen,
                        new Rect(0, rc.Top, textView.ActualWidth, rc.Height));
                }
            }
        }
    }
}