using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace eShop.UWP.Converters
{
    public class StateToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is bool))
            {
                return Application.Current.Resources["TertiaryDarkBrush"] as SolidColorBrush;
            }
            
            if ((bool)value)
            {
                return Application.Current.Resources["PrimaryMediumBrush"] as SolidColorBrush;
            }

            return Application.Current.Resources["TertiaryDarkBrush"] as SolidColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
