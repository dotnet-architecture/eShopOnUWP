using System;
using Windows.UI.Xaml.Data;
using eShop.UWP.Helpers;

namespace eShop.UWP.Converters
{
    public class BoolToStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is bool))
            {
                return Constants.InactiveStateKey.GetLocalized();
            }
            
            if ((bool)value)
            {
                return Constants.ActivateStateKey.GetLocalized();
            }

            return Constants.InactiveStateKey.GetLocalized();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Constants.ActivateStateKey.Equals(value.ToString(), StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
