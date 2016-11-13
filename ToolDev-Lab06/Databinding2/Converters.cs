using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;

namespace Databinding2
{
    public class ScalarToColoursConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var r = System.Convert.ToByte(values[0]);
            byte g = System.Convert.ToByte(values[1]);
            byte b = System.Convert.ToByte(values[2]);

            return new SolidColorBrush(Color.FromRgb(r,g,b));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var color = ((SolidColorBrush)value).Color;

            return new object[] { (double)color.R, (double)color.G, (double)color.B};
        }
    }
}