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

        // TODO: Remove
        //public DateTimeOffset NonNullable(DateTimeOffset? value)
        //{
        //    // CalendarDatePicker is using Addyears(-100) as MinValue
        //    return value != null ? value.Value : DateTimeOffset.UtcNow.AddYears(-99);
        //}

        //public DateTimeOffset NonNullableMax(DateTimeOffset? value)
        //{
        //    // CalendarDatePicker is using Addyears(+100) as MaxValue
        //    return value != null ? value.Value : DateTimeOffset.UtcNow.AddYears(+99);
        //}
    }
}
