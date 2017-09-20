using System;
using System.Diagnostics;
using eShop.UWP.Services;
using eShop.UWP.ViewModels.Base;
using eShop.UWP.ViewModels.Login;
using Microsoft.HockeyApp;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace eShop.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private readonly Lazy<ActivationService> _activationService;

        private ActivationService ActivationService => _activationService.Value;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            //Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.PreferredLaunchViewSize = new Size(1200, 800);

            HockeyClient.Current.Configure(Constants.HockeyAppID);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (!e.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(e);
            }

            try
            {
                var voiceCommandDefinition = await Package.Current.InstalledLocation.GetFileAsync(@"VoiceCommandDefinition.xml");
                await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(voiceCommandDefinition);

                if (Current.Resources["Locator"] is ViewModelLocator locator)
                {
                    await locator.ShellViewModel.UpdateCatalogTypePhraseList();
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Voice Command Definition (VCD) file not installed.");
            }


            Window.Current.Activate();
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(960, 670));
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(LoginViewModel));
        }
    }
}
