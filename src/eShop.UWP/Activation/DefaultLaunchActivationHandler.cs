using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace eShop.UWP.Activation
{
    internal class DefaultLaunchActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
    {
        protected override async Task HandleInternalAsync(LaunchActivatedEventArgs args, Type defaultNavItem)
        {
            // When the navigation stack isn't restored navigate to the first page,
            // configuring the new page by passing required information as a navigation
            // parameter
            NavigationService.Navigate(defaultNavItem.FullName, args.Arguments);

            await Task.CompletedTask;
        }

        protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
        {
            // None of the ActivationHandlers has handled the app activation
            return NavigationService.Frame.Content == null;
        }
    }
}

