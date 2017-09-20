using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace eShop.UWP.Behaviors
{
    public static class TitleBar
    {
        public static readonly DependencyProperty ForegroundColorProperty =
            DependencyProperty.RegisterAttached("ForegroundColor", typeof(Color),
            typeof(TitleBar),
            new PropertyMetadata(null, OnForegroundColorPropertyChanged));

        public static readonly DependencyProperty ButtonForegroundColorProperty =
            DependencyProperty.RegisterAttached("ButtonForegroundColor", typeof(Color),
            typeof(TitleBar),
            new PropertyMetadata(null, OnButtonForegroundColorPropertyChanged));

        public static readonly DependencyProperty ButtonBackgroundColorProperty =
            DependencyProperty.RegisterAttached("ButtonBackgroundColor", typeof(Color),
            typeof(TitleBar),
            new PropertyMetadata(null, OnButtonBackgroundColorPropertyChanged));

        public static readonly DependencyProperty ButtonInactiveForegroundColorProperty =
           DependencyProperty.RegisterAttached("ButtonInactiveForegroundColor", typeof(Color),
           typeof(TitleBar),
           new PropertyMetadata(null, OnButtonInactiveForegroundColorPropertyChanged));

        public static readonly DependencyProperty ButtonInactiveBackgroundColorProperty =
            DependencyProperty.RegisterAttached("ButtonInactiveBackgroundColor", typeof(Color),
            typeof(TitleBar),
            new PropertyMetadata(null, OnButtonInactiveBackgroundColorPropertyChanged));

        public static Color GetForegroundColor(DependencyObject d)
        {
            return (Color)d.GetValue(ForegroundColorProperty);
        }

        public static void SetForegroundColor(DependencyObject d, Color value)
        {
            d.SetValue(ForegroundColorProperty, value);
        }

        public static Color GetButtonForegroundColor(DependencyObject d)
        {
            return (Color)d.GetValue(ButtonForegroundColorProperty);
        }

        public static void SetButtonForegroundColor(DependencyObject d, Color value)
        {
            d.SetValue(ButtonForegroundColorProperty, value);
        }

        public static Color GetButtonBackgroundColor(DependencyObject d)
        {
            return (Color)d.GetValue(ButtonBackgroundColorProperty);
        }

        public static void SetButtonBackgroundColor(DependencyObject d, Color value)
        {
            d.SetValue(ButtonBackgroundColorProperty, value);
        }

        public static Color GetButtonInactiveForegroundColor(DependencyObject d)
        {
            return (Color)d.GetValue(ButtonInactiveForegroundColorProperty);
        }

        public static void SetButtonInactiveForegroundColor(DependencyObject d, Color value)
        {
            d.SetValue(ButtonInactiveForegroundColorProperty, value);
        }

        public static Color GetButtonInactiveBackgroundColor(DependencyObject d)
        {
            return (Color)d.GetValue(ButtonInactiveBackgroundColorProperty);
        }

        public static void SetButtonInactiveBackgroundColor(DependencyObject d, Color value)
        {
            d.SetValue(ButtonInactiveBackgroundColorProperty, value);
        }

        private static void OnForegroundColorPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var color = (Color)e.NewValue;
            var titleBar = GetTitleBar();
            if (titleBar != null) titleBar.ForegroundColor = color;
        }

        private static void OnButtonForegroundColorPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var color = (Color)e.NewValue;
            var titleBar = GetTitleBar();
            if (titleBar != null) titleBar.ButtonForegroundColor = color;
        }

        private static void OnButtonBackgroundColorPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var color = (Color)e.NewValue;
            var titleBar = GetTitleBar();
            if (titleBar != null) titleBar.ButtonBackgroundColor = color;
        }

        private static void OnButtonInactiveForegroundColorPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var color = (Color)e.NewValue;
            var titleBar = GetTitleBar();
            if (titleBar != null) titleBar.ButtonInactiveForegroundColor = color;
        }

        private static void OnButtonInactiveBackgroundColorPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var color = (Color)e.NewValue;
            var titleBar = GetTitleBar();
            if (titleBar != null) titleBar.ButtonInactiveBackgroundColor = color;
        }

        private static ApplicationViewTitleBar GetTitleBar()
        {
            return !ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView")
                ? null
                : ApplicationView.GetForCurrentView().TitleBar;
        }
    }
}
