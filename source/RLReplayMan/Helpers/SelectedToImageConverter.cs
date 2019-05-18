using System;
using System.Globalization;
using System.Windows.Data;

namespace RLReplayMan
{
    class BoolToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";

            var boolVal = (bool)value;

            if (boolVal)
                return "Images/bookmark_remove.png";
            else
                return "Images/bookmark.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
