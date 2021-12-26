using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace Manuals.Converters
{
    public class ImageNameToSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty((string)value))
            {
                return "";
            }
            return Path.Combine(Constants.ProductImagesFolder, (string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        //public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //{

        //}
    }
}
