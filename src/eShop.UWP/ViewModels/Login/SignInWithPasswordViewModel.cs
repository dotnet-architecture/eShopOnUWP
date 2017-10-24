using System;
using System.Threading.Tasks;
using System.Windows.Input;
using eShop.Cortana;
using eShop.UWP.Helpers;
using eShop.UWP.ViewModels.Base;
using eShop.UWP.ViewModels.Shell;
using GalaSoft.MvvmLight.Command;
using Windows.ApplicationModel;
using Windows.Security.Credentials;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml.Input;

namespace eShop.UWP.ViewModels.Login
{
    public class SignInWithPasswordViewModel : CustomViewModelBase
    {
        private string _userName;
        private string _password;
        private bool _keyCredentialIsSupported;
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

        public CatalogVoiceCommand CatalogVoiceCommand { get; set; }

        public bool CanSignIn => !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password);

        public bool CanSetupHello => _keyCredentialIsSupported && !string.IsNullOrEmpty(UserName);

        public ICommand SignInCommand => new RelayCommand(SignIn);

        public ICommand SetupHelloCommand => new RelayCommand(SetupHello);

        public ICommand KeyDownCommand => new RelayCommand<KeyRoutedEventArgs>(KeyDown);

        public async override void OnActivate(object parameter, bool isBack)
        {
            base.OnActivate(parameter, isBack);

            _keyCredentialIsSupported = await KeyCredentialManager.IsSupportedAsync();

            _versionPackage = string.Format(Constants.AuthenticationVersionCustomKey.GetLocalized(), GetAppVersion());

            _userName = Constants.AuthenticationUsernameDefaultCustomKey.GetLocalized();
            _password = Constants.AuthenticationPasswordDefaultCustomKey.GetLocalized();
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

            UserName = string.Empty;
            Password = string.Empty;

            // TODO: Add here the logic to login with your authentication service.
            ShellStartup.Start(CatalogVoiceCommand);
            CatalogVoiceCommand = null;
        }

        private async void SetupHello()
        {
            var publicKey = await CreatePassportKeyCredentialAsync();
            if (publicKey != null)
            {
                ApplicationData.Current.LocalSettings.Values[Constants.HelloUserIdKey] = UserName;
                HashAlgorithmProvider hashProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
                IBuffer publicKeyHash = hashProvider.HashData(publicKey);
                ApplicationData.Current.LocalSettings.Values[Constants.HelloPublicKeyHintKey] = CryptographicBuffer.EncodeToBase64String(publicKeyHash);

                ShellStartup.Start(CatalogVoiceCommand);
                CatalogVoiceCommand = null;
            }
        }

        private async Task<IBuffer> CreatePassportKeyCredentialAsync()
        {
            // Create a new KeyCredential for the user on the device.
            var keyCreationResult = await KeyCredentialManager.RequestCreateAsync(UserName, KeyCredentialCreationOption.ReplaceExisting);
            if (keyCreationResult.Status == KeyCredentialStatus.Success)
            {
                // User has autheniticated with Windows Hello and the key credential is created.
                var userKey = keyCreationResult.Credential;
                return userKey.RetrievePublicKey();
            }
            else if (keyCreationResult.Status == KeyCredentialStatus.NotFound)
            {
                var message = new MessageDialog(Constants.HelloSettingsNotFoundKey.GetLocalized());
                await message.ShowAsync();
                return null;
            }
            else if (keyCreationResult.Status == KeyCredentialStatus.UnknownError)
            {
                var message = new MessageDialog(Constants.HelloCreateCredencialErrorKey.GetLocalized());
                await message.ShowAsync();
                return null;
            }

            return null;
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
