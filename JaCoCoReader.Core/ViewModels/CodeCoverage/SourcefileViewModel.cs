using System;
using System.IO;
using System.Linq;
using System.Windows.Media;
using JaCoCoReader.Core.Models.CodeCoverage;
using JaCoCoReader.Core.UI;

namespace JaCoCoReader.Core.ViewModels.CodeCoverage
{
    public class SourcefileViewModel : NodeViewModel<Sourcefile>, IFolderNodeViewModel
    {
        public SourcefileViewModel(Sourcefile model) : base(model)
        {
        }

        public override string Description
        {
            get { return FileName; }
        }

        private string _text;
        public string Text
        {
            get
            {
                if (string.IsNullOrEmpty(_text))
                {
                    try
                    {
                        _text = File.ReadAllText(Model.Name);
                    }
                    catch (Exception ex)
                    {
                        _text = ex.Message;
                    }
                }
                return _text;
            }
        }

        private string _fileName;
        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(_fileName))
                {
                    _fileName = Path.GetFileName(Model.Name);
                }
                return _fileName;
            }
        }

        private LineHitDictionary _linesHit;

        public LineHitDictionary LinesHit
        {
            get
            {
                if (_linesHit == null)
                {
                    _linesHit = new LineHitDictionary(Model.Lines);
                }
                return _linesHit;
            }
        }


        public int TotalLines
        {
            get
            {
                return CoveredLines + MissedLines;
            }
        }

        private int _coveredLines = -1;

        public int CoveredLines
        {
            get
            {
                if (_coveredLines < 0)
                {
                    CountLines();
                }
                return _coveredLines;
            }
        }

        private int _missedLines = -1;
        private LineBrushCollection _brushLines;

        public int MissedLines
        {
            get
            {
                if (_missedLines < 0)
                {
                    CountLines();
                }
                return _missedLines;
            }
        }


        public double CoveredLinesPercentage
        {
            get { return CoveredLines / (double)TotalLines; }
        }


        public double MissedLinesPercentage
        {
            get { return MissedLines / (double)TotalLines; }
        }

        private void CountLines()
        {
            int missedLines = 0;
            int coveredLines = 0;
            foreach (Line line in Model.Lines)
            {
                if (line.Ci > 0)
                {
                    coveredLines++;
                }
                else
                {
                    missedLines++;
                }
            }
            _missedLines = missedLines;
            _coveredLines = coveredLines;
        }

        public Brush Background
        {
            get
            {
                if (MissedLines > 0)
                {
                    return Brushes.DarkRed;
                }
                if (CoveredLines > 0)
                {
                    return Brushes.DarkGreen;
                }
                return Colors.DefaultBackground;
            }
        }

        public LineBrushCollection BrushLines
        {
            get
            {
                if (_brushLines == null)
                {
                    _brushLines = new LineBrushCollection();
                    foreach (Line line in Model.Lines)
                    {
                        Brush brush;
                        if (line.Mi > 0)
                        {
                            brush = Brushes.Red;
                        }
                        else if (line.Ci > 0)
                        {
                            brush = Brushes.Green;
                        }
                        else
                        {
                            brush = Colors.DefaultBackground;
                        }
                        _brushLines.Add(new LineBrush(brush, line.Nr * 1000));
                    }
                }
                return _brushLines;
            }
        }

        public int LineHeight
        {
            get
            {
                int line = Model.Lines.Max(i => i.Nr);
                return line * 1000;
            }
        }
    }
}
