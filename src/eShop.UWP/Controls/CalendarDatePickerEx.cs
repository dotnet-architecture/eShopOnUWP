using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace eShop.UWP.Controls
{
    public class CalendarDatePickerEx : CalendarDatePicker
    {
        public DateTimeOffset? Min
        {
            get { return (DateTimeOffset?)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        public DateTimeOffset? Max
        {
            get { return (DateTimeOffset?)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        private static void MinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CalendarDatePickerEx;
            var date = e.NewValue as DateTimeOffset?;
            control.MinDate = date == null ? DateTimeOffset.UtcNow.AddYears(-99) : date.Value;
        }

        private static void MaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CalendarDatePickerEx;
            var date = e.NewValue as DateTimeOffset?;
            control.MaxDate = date == null ? DateTimeOffset.UtcNow.AddYears(+99) : date.Value;
        }

        public static readonly DependencyProperty MinProperty = DependencyProperty.Register("Min", typeof(DateTimeOffset?), typeof(CalendarDatePickerEx), new PropertyMetadata(null, MinChanged));
        public static readonly DependencyProperty MaxProperty = DependencyProperty.Register("Max", typeof(DateTimeOffset?), typeof(CalendarDatePickerEx), new PropertyMetadata(null, MaxChanged));
    }
}
