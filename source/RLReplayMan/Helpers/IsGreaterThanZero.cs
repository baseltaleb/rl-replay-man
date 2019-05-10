using System;
using System.Windows.Data;

namespace RLReplayMan
{
    public class IsGreaterThanZero : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                value = 0;
            return (int)value > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();//"PresenterConverter.ConvertBack() is not implemented!");
        }
        #endregion
    }
}
