using System;
using System.Globalization;
using System.Windows.Data;

namespace JaCoCoReader.Core.UI.Converters
{
    public class BooleanConverter : BaseConverter, IValueConverter
    {
        public static readonly IValueConverter Instance = new BooleanConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DoConvert(value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}