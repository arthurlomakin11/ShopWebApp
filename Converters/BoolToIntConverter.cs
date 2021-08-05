using System;
using System.Globalization;
using System.Windows.Data;

namespace ShopWebApp.Converters
{
    class BoolToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? BoolValue = (bool?)value;
            if(!BoolValue.HasValue || BoolValue.Value == false)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
