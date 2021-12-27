using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Xamarin.Forms;
using static Manuals.Constants;

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
            return Path.Combine(GetLocalFolder(FileType.ProductImage), (string)value);
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
