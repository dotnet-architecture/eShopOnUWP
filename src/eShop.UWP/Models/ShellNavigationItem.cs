using eShop.UWP.Helpers;
using eShop.UWP.ViewModels.Base;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace eShop.UWP.Models
{
    public class ShellNavigationItem : CustomViewModelBase
    {
        private bool _isSelected;
        private Visibility _selectedVis = Visibility.Collapsed;
        private SolidColorBrush _selectedForeground = null;

        public ShellNavigationItem(string name, string symbol, string viewModelName)
        {
            Label = name;
            Symbol = symbol;
            ViewModelName = viewModelName;
            ToolTipMenu = string.Format(Constants.Menu_ToolTipKey.GetLocalized(), name);

            Services.ThemeSelectorService.OnThemeChanged += (s, e) => { if (!IsSelected) SelectedForeground = GetStandardTextColorBrush(); };
        }

        public Visibility SelectedVis
        {
            get => _selectedVis;
            set => Set(ref _selectedVis, value);
        }

        public SolidColorBrush SelectedForeground
        {
            get => _selectedForeground ?? (_selectedForeground = GetStandardTextColorBrush());
            set => Set(ref _selectedForeground, value);
        }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                Set(ref _isSelected, value);
                SelectedVis = value ? Visibility.Visible : Visibility.Collapsed;
                SelectedForeground = value
                    ? Application.Current.Resources["PrimaryMediumBrush"] as SolidColorBrush
                    : GetStandardTextColorBrush();
            }
        }

        public string Label { get; set; }
        public string Symbol { get; set; }
        public string ViewModelName { get; set; }
        public string ToolTipMenu { get; set; }

        private SolidColorBrush GetStandardTextColorBrush()
        {
            var brush = Application.Current.Resources["SystemControlForegroundBaseHighBrush"] as SolidColorBrush;

            if (!Services.ThemeSelectorService.IsLightThemeEnabled)
            {
                brush = Application.Current.Resources["SystemControlForegroundAltHighBrush"] as SolidColorBrush;
            }

            return brush;
        }
    }
}
