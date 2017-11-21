using System.Linq;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using Brushes = JaCoCoReader.Core.ViewModels.Brushes;

namespace JaCoCoReader.UI.Controls
{
    public class TestsBackgroundRenderer : IBackgroundRenderer
    {
        static readonly Pen Pen;

        readonly CodeCoverageTextEditor _editor;

        static TestsBackgroundRenderer()
        {
            SolidColorBrush blackBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)); blackBrush.Freeze();
            Pen = new Pen(blackBrush, 0.0);
        }

        public TestsBackgroundRenderer(CodeCoverageTextEditor editor)
        {
            this._editor = editor;
        }

        public KnownLayer Layer
        {
            get { return KnownLayer.Background; }
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_editor?.LinesHit == null
                || !_editor.ShowLinesHit)
            {
                return;
            }

            foreach (var v in textView.VisualLines)
            {
                var rc = BackgroundGeometryBuilder.GetRectsFromVisualSegment(textView, v, 0, 1000).First();

                int linenum = v.FirstDocumentLine.LineNumber;

                bool hit;
                if (_editor.LinesHit.TryGetValue(linenum, out hit))
                {
                    Brush brush;
                    if (hit)
                    {
                        brush = Brushes.HitBackground;
                    }
                    else
                    {
                        brush = Brushes.MissedBackground;
                    }

                    drawingContext.DrawRectangle(brush, Pen,
                        new Rect(0, rc.Top, textView.ActualWidth, rc.Height));
                }
            }
        }
    }
}