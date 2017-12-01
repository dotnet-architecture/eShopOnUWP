using System;
using System.Windows.Input;
using System.Threading.Tasks;

using Windows.Storage.Streams;
using Windows.Security.Credentials;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

using Microsoft.Practices.ServiceLocation;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using eShop.UWP.Services;
using eShop.UWP.Activation;
using eShop.UWP.Authentication;
using eShop.UWP.Helpers;

namespace eShop.UWP.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private ActivationState _activationState;

        private NavigationServiceEx NavigationService => ServiceLocator.Current.GetInstance<NavigationServiceEx>();

        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { Set(ref _isBusy, value); }
        }

        private bool _isLoginWithPassword = false;
        public bool IsLoginWithPassword
        {
            get { return _isLoginWithPassword; }
            set { Set(ref _isLoginWithPassword, value); }
        }

        private bool _isLoginWithWindowsHello = false;
        public bool IsLoginWithWindowsHello
        {
            get { return _isLoginWithWindowsHello; }
            set { Set(ref _isLoginWithWindowsHello, value); }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { Set(ref _userName, value); }
        }

        private string _password = "UserPassword";
        public string Password
        {
            get { return _password; }
            set { Set(ref _password, value); }
        }

        public string VersionPackage => string.Format(Constants.AuthenticationVersionCustomKey.GetLocalized(), AppSettings.Current.Version);

        public ICommand ShowLoginWithPasswordCommand => new RelayCommand(ShowLoginWithPassword);
        public ICommand LoginWithPasswordCommand => new RelayCommand(LoginWithPassword);
        public ICommand LoginWithHelloCommand => new RelayCommand(LoginWithHello);

        public async Task InitializeAsync(ActivationState activationState)
        {
            _activationState = activationState;

            UserName = await GetUserNameAsync();
            IsLoginWithWindowsHello = IsWindowsHelloEnabled(UserName);
            IsLoginWithPassword = !IsLoginWithWindowsHello;
            IsBusy = false;
        }

        private async Task<string> GetUserNameAsync()
        {
            var contact = await ContactHelper.CreateContactFromCurrentUserAsync();
            var userName = AppSettings.Current.UserName;
            if (userName == null)
            {
                userName = contact.DisplayName;
            }
            return userName;
        }

        private void ShowLoginWithPassword()
        {
            IsLoginWithWindowsHello = false;
            IsLoginWithPassword = true;
        }

        public void Login()
        {
            if (IsLoginWithPassword)
            {
                LoginWithPassword();
            }
            else
            {
                LoginWithHello();
            }
        }

        public async void LoginWithPassword()
        {
            IsBusy = true;
            Views.LoginView.CurrentEffectMode = Views.LoginView.EffectMode.Disabled;
            var result = ValidateInput();
            if (result.IsOk)
            {
                if (await SignInWithPasswordAsync(UserName, Password))
                {
                    if (!IsWindowsHelloEnabled(UserName))
                    {
                        await TrySetupWindowsHelloAsync(UserName);
                    }
                    AppSettings.Current.UserName = UserName;
                    EnterApplication();
                    return;
                }
            }
            await DialogBox.ShowAsync(result);
            Views.LoginView.CurrentEffectMode = Views.LoginView.EffectMode.Foreground;
            IsBusy = false;
        }

        public async void LoginWithHello()
        {
            IsBusy = true;
            Views.LoginView.CurrentEffectMode = Views.LoginView.EffectMode.Disabled;
            var result = await SignInWithWindowsHelloAsync();
            if (result.IsOk)
            {
                EnterApplication();
                return;
            }
            Views.LoginView.CurrentEffectMode = Views.LoginView.EffectMode.Foreground;
            await DialogBox.ShowAsync(result);
            IsBusy = false;
        }

        private void EnterApplication()
        {
            NavigationService.Navigate(typeof(ShellViewModel).FullName, _activationState, mainFrame: true);
        }

        private Result ValidateInput()
        {
            if (String.IsNullOrWhiteSpace(UserName))
            {
                return Result.Error("Login error", "Please, enter a valid user name.");
            }
            if (String.IsNullOrWhiteSpace(Password))
            {
                return Result.Error("Login error", "Please, enter a valid password.");
            }
            return Result.Ok();
        }

        private async Task<bool> SignInWithPasswordAsync(string userName, string password)
        {
            var result = await AuthenticationService.AuthenticateAsync(userName, password);
            if (result.IsOk)
            {
                return true;
            }
            await DialogBox.ShowAsync(result);
            return false;
        }

        private async Task<Result> SignInWithWindowsHelloAsync()
        {
            string userName = AppSettings.Current.UserName;
            if (IsWindowsHelloEnabled(userName))
            {
                var retrieveResult = await KeyCredentialManager.OpenAsync(userName);
                if (retrieveResult.Status == KeyCredentialStatus.Success)
                {
                    var credential = retrieveResult.Credential;
                    var challengeBuffer = CryptographicBuffer.DecodeFromBase64String("challenge");
                    var result = await credential.RequestSignAsync(challengeBuffer);
                    if (result.Status == KeyCredentialStatus.Success)
                    {
                        return Result.Ok();
                    }
                    return Result.Error("Windows Hello", $"Cannot sign in with Windows Hello: {result.Status}");
                }
                return Result.Error("Windows Hello", $"Cannot sign in with Windows Hello: {retrieveResult.Status}");
            }
            return Result.Error("Windows Hello", "Windows Hello is not enabled for current user.");
        }

        private bool IsWindowsHelloEnabled(string userName)
        {
            if (!String.IsNullOrEmpty(userName))
            {
                if (userName.Equals(AppSettings.Current.UserName, StringComparison.OrdinalIgnoreCase))
                {
                    return AppSettings.Current.WindowsHelloPublicKeyHint != null;
                }
            }
            return false;
        }

        private async Task TrySetupWindowsHelloAsync(string userName)
        {
            if (await KeyCredentialManager.IsSupportedAsync())
            {
                if (await DialogBox.ShowAsync("Windows Hello", "Your device supports Windows Hello and you can use it to authenticate with the app.\r\n\r\nDo you want to enable Windows Hello for your next sign in with this user?", "Ok", "Maybe later"))
                {
                    await SetupWindowsHelloAsync(userName);
                }
                else
                {
                    await TryDeleteCredentialAccountAsync(userName);
                }
            }
        }

        private async Task SetupWindowsHelloAsync(string userName)
        {
            var publicKey = await CreatePassportKeyCredentialAsync(userName);
            if (publicKey != null)
            {
                if (await AuthenticationService.RegisterPassportCredentialWithServerAsync(publicKey))
                {
                    // When communicating with the server in the future, we pass a hash of the
                    // public key in order to identify which key the server should use to verify the challenge.
                    var hashProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
                    var publicKeyHash = hashProvider.HashData(publicKey);
                    AppSettings.Current.WindowsHelloPublicKeyHint = CryptographicBuffer.EncodeToBase64String(publicKeyHash);
                }
            }
            else
            {
                await TryDeleteCredentialAccountAsync(userName);
            }
        }

        private async Task<IBuffer> CreatePassportKeyCredentialAsync(string userName)
        {
            // Create a new KeyCredential for the user on the device
            var keyCreationResult = await KeyCredentialManager.RequestCreateAsync(userName, KeyCredentialCreationOption.ReplaceExisting);

            if (keyCreationResult.Status == KeyCredentialStatus.Success)
            {
                // User has autheniticated with Windows Hello and the key credential is created
                var credential = keyCreationResult.Credential;
                return credential.RetrievePublicKey();
            }
            else if (keyCreationResult.Status == KeyCredentialStatus.NotFound)
            {
                await DialogBox.ShowAsync("Windows Hello", "To proceed, Windows Hello needs to be configured in Windows Settings (Accounts -> Sign-in options)");
            }
            else if (keyCreationResult.Status == KeyCredentialStatus.UnknownError)
            {
                await DialogBox.ShowAsync("Windows Hello Error", "The key credential could not be created. Please try again.");
            }

            return null;
        }

        const int NTE_NO_KEY = unchecked((int)0x8009000D);
        const int NTE_PERM = unchecked((int)0x80090010);

        static private async Task<bool> TryDeleteCredentialAccountAsync(string userName)
        {
            try
            {
                AppSettings.Current.WindowsHelloPublicKeyHint = null;
                await KeyCredentialManager.DeleteAsync(userName);
                return true;
            }
            catch (Exception ex)
            {
                switch (ex.HResult)
                {
                    case NTE_NO_KEY:
                        // Key is already deleted. Ignore this error.
                        break;
                    case NTE_PERM:
                        // Access is denied. Ignore this error. We tried our best.
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                        break;
                }
            }
            return false;
        }
    }
}
