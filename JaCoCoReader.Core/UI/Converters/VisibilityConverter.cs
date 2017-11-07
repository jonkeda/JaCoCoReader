using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace JaCoCoReader.Core.UI.Converters
{
    public class VisibilityConverter : BaseConverter, IValueConverter
    {
        public static readonly IValueConverter Instance = new VisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DoConvert(value, targetType, parameter, culture))
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}