using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using eShop.UWP.ViewModels.Shell;
using eShop.UWP.Views.Base;
using eShop.UWP.Services;
using Microsoft.Practices.ServiceLocation;
using eShop.UWP.ViewModels;

namespace eShop.UWP.Views.Shell
{
    public sealed partial class ShellView : PageBase
    {
        private ShellViewModel ViewModel => DataContext as ShellViewModel;

        public ShellView()
        {
            InitializeComponent();
        }

        private NavigationServiceEx NavigationService => ServiceLocator.Current.GetInstance<NavigationServiceEx>();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel.Initialize(shellFrame);
        }

        private void NavigationItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                NavigationService.Navigate(typeof(SettingsViewModel).FullName);
            }
        }
    }
}
