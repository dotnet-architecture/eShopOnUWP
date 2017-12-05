using System;

using Windows.UI.Xaml;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;

using Microsoft.HockeyApp;

using eShop.UWP.ViewModels;
using eShop.UWP.Activation;

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

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.PreferredLaunchViewSize = new Size(1280, 840);

            HockeyClient.Current.Configure(Constants.HockeyAppID);
        }

        private ActivationService ActivationService => _activationService.Value;

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
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
            //return new ActivationService(this, typeof(LoginViewModel), new ActivationState(typeof(CatalogViewModel), new CatalogState()));
            return new ActivationService(this, typeof(ShellViewModel), new ActivationState(typeof(CatalogViewModel), new CatalogState()));
        }
    }
}
