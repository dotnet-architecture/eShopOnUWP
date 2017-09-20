using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace eShop.UWP.Converters
{
    public class BoolToCommandBarDisplayModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is bool))
            {
                return AppBarClosedDisplayMode.Hidden;
            }

            if ((bool)value)
            {
                return AppBarClosedDisplayMode.Compact;
            }

            return AppBarClosedDisplayMode.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
