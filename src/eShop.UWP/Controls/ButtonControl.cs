using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace eShop.UWP.Controls
{
    public class ButtonControl : Button
    {
        public string IconSourcePath
        {
            get => (string)GetValue(IconSourcePathProperty);
            set => SetValue(IconSourcePathProperty, value);
        }

        public static readonly DependencyProperty IconSourcePathProperty =
            DependencyProperty.Register("IconSourcePath", typeof(string), typeof(ButtonControl), new PropertyMetadata(null));
    }
}
