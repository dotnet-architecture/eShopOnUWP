using System;
using Windows.UI.Xaml.Data;

namespace eShop.UWP.Converters
{
    public class StateToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is bool))
            {
                return 0.2;
            }
            
            if ((bool)value)
            {
                return 1;
            }

            return 0.2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
