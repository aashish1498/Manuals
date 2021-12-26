using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace Manuals.Converters
{
    public class ImageToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var image = (ImageSource)value;
            if (image == null)
            {
                return false;
            }
            return !image.IsEmpty;
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
