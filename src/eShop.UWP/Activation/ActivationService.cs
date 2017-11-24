using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Activation;

using Microsoft.Practices.ServiceLocation;

using eShop.UWP.Services;
using eShop.UWP.Helpers;

namespace eShop.UWP.Activation
{
    // For more information on application activation see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/activation.md
    internal class ActivationService
    {
        private readonly App _app;
        private readonly Type _view;
        private readonly ActivationState _activationState;

        public ActivationService(App app, Type view, ActivationState state)
        {
            _app = app;
            _view = view;
            _activationState = state;
        }

        private SystemNavigationManager CurrentView => SystemNavigationManager.GetForCurrentView();
        private NavigationServiceEx NavigationService => ServiceLocator.Current.GetInstance<NavigationServiceEx>();

        public async Task ActivateAsync(IActivatedEventArgs activationArgs)
        {
            bool isLaunch = false;
            if (IsInteractive(activationArgs))
            {
                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                if (Window.Current.Content == null)
                {
                    isLaunch = true;

                    // Initialize things like registering background task before the app is loaded
                    await InitializeAsync();

                    // Create a Frame to act as the navigation context and navigate to the first page
                    var frame = new Frame();
                    Window.Current.Content = frame;

                    NavigationService.MainFrame = frame;
                    NavigationService.NavigationFailed += OnNavigationFailed;
                    NavigationService.Navigated += OnNavigated;

                    if (CurrentView != null)
                    {
                        CurrentView.BackRequested += OnBackRequested;
                    }
                }
            }

            ActivationState activationState = null;
            var activationHandler = GetActivationHandlers().FirstOrDefault(h => h.CanHandle(activationArgs));
            if (activationHandler != null)
            {
                activationState = await activationHandler.HandleAsync(activationArgs);
            }
            activationState = activationState ?? _activationState;

            if (IsInteractive(activationArgs))
            {
                if (isLaunch)
                {
                    NavigationService.Navigate(_view.FullName, activationState, mainFrame: true);
                }
                else
                {
                    NavigationService.Navigate(activationState.ViewModel.ToString(), activationState.Parameter);
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
