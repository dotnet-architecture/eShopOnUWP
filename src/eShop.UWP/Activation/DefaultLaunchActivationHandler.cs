using System;
using System.Threading.Tasks;

using Windows.ApplicationModel.Activation;

using Microsoft.Practices.ServiceLocation;

using eShop.UWP.Services;

namespace eShop.UWP.Activation
{
    internal class DefaultLaunchActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
    {
        private readonly Type _navView;
        private readonly ViewState _navViewState;

        public DefaultLaunchActivationHandler(Type navView, ViewState navViewState)
        {
            _navView = navView;
            _navViewState = navViewState;
        }

        private NavigationServiceEx NavigationService => ServiceLocator.Current.GetInstance<NavigationServiceEx>();

        protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
        {
            // None of the ActivationHandlers has handled the app activation
            return NavigationService.Frame?.Content == null;
        }

        protected override async Task HandleInternalAsync(LaunchActivatedEventArgs args)
        {
            // When the navigation stack isn't restored, navigate to the first page and configure
            // the new page by passing required information in the navigation parameter
            _navViewState.LaunchArguments = args.Arguments;

            if (_navView != null)
            {
                NavigationService.Navigate(_navView.FullName, _navViewState);
            }

            await Task.CompletedTask;
        }
    }
}
