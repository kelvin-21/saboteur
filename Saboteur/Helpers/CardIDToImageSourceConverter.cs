﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Saboteur.Helpers
{
    public class CardIDToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int cardID)
            {
                string ImageSource = "/Resources/Card/" + cardID + ".jpg";
                return ImageSource;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
