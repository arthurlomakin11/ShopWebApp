using System;
using System.Globalization;
using System.Windows.Data;

namespace ShopWebApp.Converters
{
    public class ColorConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Windows.Media.Color oldColor = (System.Windows.Media.Color)value;
            System.Drawing.Color myColor = System.Drawing.Color.FromArgb(oldColor.A, oldColor.R, oldColor.G, oldColor.B);
            return myColor;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Drawing.Color oldColor = (System.Drawing.Color)value;
            System.Windows.Media.Color myColor = System.Windows.Media.Color.FromArgb(oldColor.A, oldColor.R, oldColor.G, oldColor.B);
            return myColor;
        }
    }
}
