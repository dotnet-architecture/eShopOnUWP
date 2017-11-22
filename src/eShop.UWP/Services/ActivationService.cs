using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Activation;

using eShop.UWP.Activation;
using eShop.UWP.ViewModels;
using eShop.UWP.Helpers;

namespace eShop.UWP.Services
{
    // For more information on application activation see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/activation.md
    internal class ActivationService
    {
        private readonly App _app;
        private readonly Type _defaultNavItem;
        private readonly ViewState _navParameter;
        private readonly UIElement _shell;

        public ActivationService(App app, Type defaultNavItem, ViewState navParameter, UIElement shell)
        {
            _app = app;
            _defaultNavItem = defaultNavItem;
            _navParameter = navParameter ?? new ViewState();
            _shell = shell ?? new Frame();
        }

        private ViewModelLocator Locator => Application.Current.Resources["Locator"] as ViewModels.ViewModelLocator;

        private NavigationServiceEx NavigationService => Locator.NavigationService;

        public async Task ActivateAsync(object activationArgs)
        {
            if (IsInteractive(activationArgs))
            {
                // Initialize things like registering background task before the app is loaded
                await InitializeAsync();

                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                if (Window.Current.Content == null)
                {
                    // Create a Frame to act as the navigation context and navigate to the first page
                    Window.Current.Content = _shell;

                    NavigationService.NavigationFailed += OnNavigationFailed;
                    NavigationService.Navigated += OnNavigated;

                    if (SystemNavigationManager.GetForCurrentView() != null)
                    {
                        SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
                    }
                }
            }

            var activationHandler = GetActivationHandlers().FirstOrDefault(h => h.CanHandle(activationArgs));

            if (activationHandler != null)
            {
                await activationHandler.HandleAsync(activationArgs);
            }

            if (IsInteractive(activationArgs))
            {
                var defaultHandler = new DefaultLaunchActivationHandler(_defaultNavItem, _navParameter);
                if (defaultHandler.CanHandle(activationArgs))
                {
                    await defaultHandler.HandleAsync(activationArgs);
                }

                // Ensure the current window is active
                Window.Current.Activate();

                // Tasks after activation
                await StartupAsync();
            }
        }

        private async Task InitializeAsync()
        {
            Singleton<LiveTileService>.Instance.EnableQueue();
            await Task.CompletedTask;
        }

        private async Task StartupAsync()
        {
            Singleton<LiveTileService>.Instance.TileUpdate();
            await Task.CompletedTask;
        }

        private IEnumerable<ActivationHandler> GetActivationHandlers()
        {
            yield return Singleton<LiveTileService>.Instance;
            yield return Singleton<ToastNotificationsService>.Instance;
            yield return Singleton<VoiceCommandActivationService>.Instance;
        }

        private bool IsInteractive(object args)
        {
            return args is IActivatedEventArgs;
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw e.Exception;
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = NavigationService.CanGoBack ?
                AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
                e.Handled = true;
            }
        }
    }
}
