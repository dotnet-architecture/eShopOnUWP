using System;
using System.Threading.Tasks;

using eShop.UWP.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace eShop.UWP.Views
{
    public sealed partial class SettingsView : Page
    {
        public SettingsView()
        {
            InitializeComponent();
            SizeChanged += OnSizeChanged;
        }

        private SettingsViewModel ViewModel => DataContext as SettingsViewModel;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel.Initialize();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 640)
            {
                VisualStateManager.GoToState(this, "Wide", true);
            }
            else
            {
                if (Window.Current.Bounds.Width > 640)
                {
                    VisualStateManager.GoToState(this, "Narrow", true);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Overlay", true);
                }
            }
        }
    }
}
