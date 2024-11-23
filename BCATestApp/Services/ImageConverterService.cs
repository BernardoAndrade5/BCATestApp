using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCATestApp.Services
{
    public class ImageConverterService : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isFavorite = (bool)value;
            return isFavorite ? ImageSource.FromFile("heart_fill.png") : ImageSource.FromFile("heart_no_fill.png"); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null; 
        }
    }  
}
