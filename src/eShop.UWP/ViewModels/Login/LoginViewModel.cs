using System;
using System.Windows.Input;

using Windows.System;
using Windows.Storage;
using Windows.UI.Xaml.Input;
using Windows.ApplicationModel;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using eShop.UWP.Helpers;

namespace eShop.UWP.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _userName;
        private string _password;
        private string _versionPackage;

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

        public void Initialize()
        {
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

            //UserName = string.Empty;
            //Password = string.Empty;

            // TODO: Add here the logic to login with your authentication service.
            Views.ShellView.Startup();
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
