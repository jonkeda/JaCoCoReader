using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using JaCoCoReader.Models;
using JaCoCoReader.UI;
using JaCoCoReader.UI.Controls;

namespace JaCoCoReader.ViewModels
{
    public class LineBrush
    {
        public LineBrush(Brush brush, int y)
        {
            Brush = brush;
            Y = y;
        }

        public Brush Brush
        {
            get;
        }

        public int Y
        {
            get;
        }

    }
    public class LineBrushCollection : Collection<LineBrush>
    { }

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
                    LineHitDictionary linesHit = new LineHitDictionary();
                    foreach (Line line in Model.Lines)
                    {
                        if (!linesHit.ContainsKey(line.Nr))
                        {
                            if (line.Mi > 0)
                            {
                                linesHit.Add(line.Nr, false);
                            }
                            else
                            {
                                linesHit.Add(line.Nr, true);
                            }
                        }
                    }
                    _linesHit = linesHit;
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
                    var counter = Model.Counters.FirstOrDefault();
                    if (counter != null)
                    {
                        _coveredLines = counter.Covered;
                    }
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
                    var counter = Model.Counters.FirstOrDefault();
                    if (counter != null)
                    {
                        _missedLines = counter.Missed;
                    }
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

        public Brush Background
        {
            get
            {
                if (MissedLines > 0)
                {
                    return CoverageBackgroundRenderer.MissedBackground;
                }
                if (CoveredLines > 0)
                {
                    return CoverageBackgroundRenderer.HitBackground;
                }
                return CoverageBackgroundRenderer.DefaultBackground;
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
                            brush = CoverageBackgroundRenderer.DefaultBackground;
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
