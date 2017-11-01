using System.Windows.Media;

namespace JaCoCoReader.Core.ViewModels
{
    public static class Colors
    {
        public static readonly SolidColorBrush MissedBackground;
        public static readonly SolidColorBrush HitBackground;
        public static readonly SolidColorBrush DefaultBackground;
        static Colors()
        {
            MissedBackground = new SolidColorBrush(Color.FromRgb(0xff, 0xdd, 0xdd)); MissedBackground.Freeze();
            HitBackground = new SolidColorBrush(Color.FromRgb(0xdd, 0xff, 0xdd)); HitBackground.Freeze();
            DefaultBackground = Brushes.Transparent;

            SolidColorBrush blackBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)); blackBrush.Freeze();
            //Pen = new Pen(blackBrush, 0.0);
        }

    }
}
