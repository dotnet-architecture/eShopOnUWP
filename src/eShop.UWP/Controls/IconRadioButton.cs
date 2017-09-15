using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace eShop.UWP.Controls
{
    public class IconRadioButton : RadioButton
    {
        public static readonly DependencyProperty PathIconProperty =
            DependencyProperty.Register("PathIcon", typeof(string), typeof(IconRadioButton), new PropertyMetadata(string.Empty));

        public string PathIcon
        {
            get => (string)GetValue(PathIconProperty);
            set => SetValue(PathIconProperty, value);
        }
    }
}
