using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace eCommerce.MAUI.Converters
{
    public class BoolToBogoConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isBogo && isBogo)
            {
                return "BOGO";
            }
            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
