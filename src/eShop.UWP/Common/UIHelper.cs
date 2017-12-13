using System;

using Windows.UI.Xaml;

namespace eShop.UWP
{
    public class UIHelper
    {
        static public UIHelper Current { get; private set; }

        static UIHelper()
        {
            Current = new UIHelper();
        }

        public Visibility Visible(bool? value)
        {
            return value == true ? Visibility.Visible : Visibility.Collapsed;
        }

        public Visibility Collapsed(bool? value)
        {
            return value == false ? Visibility.Visible : Visibility.Collapsed;
        }

        public string Currency(double? value)
        {
            value = value ?? 0;
            return value.Value.ToString("0.00");
        }
    }
}
