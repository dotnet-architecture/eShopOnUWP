using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace eShop.UWP.Controls
{
    public class ToggleButtonControl : ToggleButton
    {
        public string PathSource
        {
            get => (string)GetValue(PathSourceProperty);
            set => SetValue(PathSourceProperty, value);
        }

        public static readonly DependencyProperty PathSourceProperty =
            DependencyProperty.Register("PathSource", typeof(string), typeof(ToggleButtonControl), new PropertyMetadata(string.Empty));
    }
}
