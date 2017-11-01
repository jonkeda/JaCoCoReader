﻿using System.Windows.Media;

namespace JaCoCoReader.Core.ViewModels
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
}