using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShop.UWP.Activation;
using eShop.UWP.Helpers;
using eShop.UWP.Views.Login;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace eShop.UWP.Services
{
    //For more information on application activation see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/activation.md
    internal class ActivationService
    {
        private readonly App _app;
        private readonly UIElement _shell;
        private readonly Type _defaultNavItem;

        private NavigationServiceEx _navigationService => Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationServiceEx>();

        public bool IsAuthenticated => _navigationService.Frame.CurrentSourcePageType != null && _navigationService.Frame.CurrentSourcePageType != typeof(LoginView);


        public ActivationService(App app, Type defaultNavItem, UIElement shell = null)
        {
            _app = app;
            _shell = shell ?? new Frame();
            _defaultNavItem = defaultNavItem;
        }

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
                    _navigationService.Frame.NavigationFailed += (sender, e) =>
                    {
                        throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
                    };

                    if (SystemNavigationManager.GetForCurrentView() != null)
                    {
                        SystemNavigationManager.GetForCurrentView().BackRequested += OnAppViewBackButtonRequested;
                    }
                }
            }

            var activationHandler = GetActivationHandlers()
                                                .FirstOrDefault(h => h.CanHandle(activationArgs));

            if (activationHandler != null)
            {
                await activationHandler.HandleAsync(activationArgs, _defaultNavItem);
            }

            if (IsInteractive(activationArgs))
            {
                var defaultHandler = new DefaultLaunchActivationHandler();
                if (defaultHandler.CanHandle(activationArgs))
                {
                    await defaultHandler.HandleAsync(activationArgs, _defaultNavItem);
                }

                // Ensure the current window is active
                Window.Current.Activate();

                // Tasks after activation
                await StartupAsync();
            }
        }

        private async Task InitializeAsync()
        {
            await ThemeSelectorService.InitializeAsync();
            await Singleton<LiveTileService>.Instance.EnableQueueAsync();
            await Task.CompletedTask;
        }

        private async Task StartupAsync()
        {
            ThemeSelectorService.SetRequestedTheme();
            Singleton<LiveTileService>.Instance.TileUpdate();
            await Task.CompletedTask;
        }

        private IEnumerable<ActivationHandler> GetActivationHandlers()
        {
            yield return Singleton<LiveTileService>.Instance;
            yield return Singleton<ToastNotificationsService>.Instance;
            yield return Singleton<ProtocolService>.Instance;
            yield return Singleton<VoiceCommandActivationService>.Instance;
        }

        private bool IsInteractive(object args)
        {
            return args is IActivatedEventArgs;
        }

        private void OnAppViewBackButtonRequested(object sender, BackRequestedEventArgs e)
        {
            if (_navigationService.CanGoBack)
            {
                _navigationService.GoBack();
                e.Handled = true;
            }
        }
    }
}

