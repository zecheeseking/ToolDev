using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using SharpDX;

namespace ToolDev_IvyGenerator.Converters
{
    public class FloatToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float f = float.Parse(value.ToString());

            return f;
        }
    }
}