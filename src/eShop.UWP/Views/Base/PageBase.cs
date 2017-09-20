using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using eShop.UWP.ViewModels.Base;

namespace eShop.UWP.Views.Base
{
    public class PageBase : Page
    {
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var customViewModelBase = DataContext as CustomViewModelBase;
            customViewModelBase?.CleanBackStack();
            customViewModelBase?.OnActivate(e.Parameter, e.NavigationMode == NavigationMode.Back);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            (DataContext as CustomViewModelBase)?.OnDeactivate();
        }
    }
}
