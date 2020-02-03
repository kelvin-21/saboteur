using System;
using System.Windows;
using System.Windows.Data;

namespace Saboteur.Helpers
{
    public class BoolToStringConverter : BoolToValueConverter<String> { }
    public class BoolToVisibilityConverter : BoolToValueConverter<Visibility> { }

    #region Generic Boolean Value Converter 
    // Reference: http://geekswithblogs.net/codingbloke/archive/2010/05/28/a-generic-boolean-value-converter.aspx
    public class BoolToValueConverter<T> : IValueConverter
    {
        public T FalseValue { get; set; }
        public T TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return FalseValue;
            else
                return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null ? value.Equals(TrueValue) : false;
        }
    }
    #endregion
}
