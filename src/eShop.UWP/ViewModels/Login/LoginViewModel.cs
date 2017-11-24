using System;
using System.Windows.Input;

using Windows.System;
using Windows.Storage;
using Windows.UI.Xaml.Input;
using Windows.ApplicationModel;

using Microsoft.Practices.ServiceLocation;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using eShop.UWP.Helpers;
using eShop.UWP.Activation;
using eShop.UWP.Services;

namespace eShop.UWP.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private ActivationState _activationState = null;

        private string _userName;
        private string _password;
        private string _versionPackage;

        private NavigationServiceEx NavigationService => ServiceLocator.Current.GetInstance<NavigationServiceEx>();

        public string UserName
        {
            get => _userName;
            set
            {
                Set(ref _userName, value);
                RaisePropertyChanged(() => CanSignIn);
                RaisePropertyChanged(() => CanSetupHello);
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                Set(ref _password, value);
                RaisePropertyChanged(() => CanSignIn);
            }
        }

        public string VersionPackage
        {
            get => _versionPackage;
            set
            {
                Set(ref _versionPackage, value);
            }
        }

        public bool CanSignIn => !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password);

        // TODO: 
        public bool CanSetupHello => false; // _keyCredentialIsSupported && !string.IsNullOrEmpty(UserName);

        public ICommand SignInCommand => new RelayCommand(SignIn);

        public ICommand SetupHelloCommand => new RelayCommand(SetupHello);

        public ICommand KeyDownCommand => new RelayCommand<KeyRoutedEventArgs>(KeyDown);

        public void Initialize(ActivationState activationState)
        {
            _activationState = activationState;

            VersionPackage = string.Format(Constants.AuthenticationVersionCustomKey.GetLocalized(), GetAppVersion());

            UserName = Constants.AuthenticationUsernameDefaultCustomKey.GetLocalized();
            Password = Constants.AuthenticationPasswordDefaultCustomKey.GetLocalized();
        }

        public static string GetAppVersion()
        {
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        private void SignIn()
        {
            ApplicationData.Current.LocalSettings.Values[Constants.HelloUserIdKey] = UserName;

            // TODO: Add here the logic to login with your authentication service.
            //NavigationService.Navigate(_activationState.View.ToString(), _activationState.Parameter, mainFrame: true);
            NavigationService.Navigate(typeof(ShellViewModel).FullName, _activationState, mainFrame: true);
        }

        private void SetupHello()
        {
            // TODO: 
        }

        private void KeyDown(KeyRoutedEventArgs args)
        {
            if (args.Key == VirtualKey.Enter && CanSignIn)
            {
                SignIn();
            }
        }
    }
}
