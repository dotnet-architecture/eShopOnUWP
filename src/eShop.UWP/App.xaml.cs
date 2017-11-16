using System;

using eShop.UWP.Views;
using eShop.UWP.ViewModels;
using eShop.UWP.Services;

using Windows.UI.Xaml;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel.Activation;

namespace eShop.UWP
{
    public sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;

        public App()
        {
            InitializeComponent();

            // Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }

        private ActivationService ActivationService => _activationService.Value;

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            //MaximizeWindowOnLoad();
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private ActivationService CreateActivationService()
        {
            // TODO: 
            //return new ActivationService(this, null, null, new LoginView());
            return new ActivationService(this, typeof(StatisticsViewModel), null, new ShellView());
        }

        private static void MaximizeWindowOnLoad()
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;

            ApplicationView.PreferredLaunchViewSize = new Windows.Foundation.Size(bounds.Width, bounds.Height);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
        }
    }
}
