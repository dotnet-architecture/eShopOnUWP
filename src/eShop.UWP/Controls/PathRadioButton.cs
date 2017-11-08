using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace eShop.UWP.Controls
{
    public class PathRadioButton : RadioButton
    {
        public string PathIcon
        {
            get => (string)GetValue(PathIconProperty);
            set => SetValue(PathIconProperty, value);
        }

        public static readonly DependencyProperty PathIconProperty = DependencyProperty.Register("PathIcon", typeof(string), typeof(PathRadioButton), new PropertyMetadata(string.Empty));
    }
}
