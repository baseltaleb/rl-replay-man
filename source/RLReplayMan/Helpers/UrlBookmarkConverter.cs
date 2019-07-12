using RLReplayMan.Properties;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RLReplayMan
{
    class UrlBookmarkConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";
            var defaultSetting = Settings.Default;

            var urlVal = (string)value;

            if (defaultSetting.Bookmarks.Contains(urlVal))
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
