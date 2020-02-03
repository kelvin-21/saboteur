using System;
using System.Globalization;
using System.Windows.Data;

namespace Saboteur.Helpers
{
    public class RadioBoolToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int interger = (int)value;
            if (interger == int.Parse(parameter.ToString()))
                return true;
            else return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}
