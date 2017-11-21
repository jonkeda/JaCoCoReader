using System.Windows.Media;

namespace JaCoCoReader.Core.ViewModels
{
    public static class Brushes
    {
        public static readonly SolidColorBrush MissedBackground;
        public static readonly SolidColorBrush HitBackground;
        public static readonly SolidColorBrush DefaultBackground;

        public static readonly SolidColorBrush NotRunBackground;

        static Brushes()
        {
            MissedBackground = new SolidColorBrush(Color.FromRgb(0xff, 0xdd, 0xdd)); MissedBackground.Freeze();
            HitBackground = new SolidColorBrush(Color.FromRgb(0xdd, 0xff, 0xdd)); HitBackground.Freeze();
            NotRunBackground = new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0xE0)); HitBackground.Freeze();
            DefaultBackground = System.Windows.Media.Brushes.Transparent;
        }
    }

    public static class Colors
    {
        public static readonly Color MissedBackground;
        public static readonly Color HitBackground;
        public static readonly Color DefaultBackground;

        public static readonly Color NotRunBackground;

        static Colors()
        {
            MissedBackground = Color.FromRgb(0xff, 0xdd, 0xdd);
            HitBackground = Color.FromRgb(0xdd, 0xff, 0xdd); 
            NotRunBackground = Color.FromRgb(0xff, 0xff, 0xE0); 
            DefaultBackground = System.Windows.Media.Colors.Transparent;
        }
    }
}
