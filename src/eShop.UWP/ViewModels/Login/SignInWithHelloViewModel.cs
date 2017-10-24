using System;
using System.Windows.Input;
using eShop.Cortana;
using eShop.UWP.Helpers;
using eShop.UWP.ViewModels.Base;
using eShop.UWP.ViewModels.Shell;
using GalaSoft.MvvmLight.Command;
using Windows.Security.Credentials;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.Storage.Streams;

namespace eShop.UWP.ViewModels.Login
{
    public class SignInWithHelloViewModel : CustomViewModelBase
    {
        private string _userId;
        private string _publicKeyHint;
        private string _greetingCustom;
        private string _waitingSignIn;

        public string WaitingSignIn
        {
            get => _waitingSignIn;
            set => Set(ref _waitingSignIn, value);
        }

        public string GreetingCustom
        {
            get => _greetingCustom;
            set => Set(ref _greetingCustom, value);
        }

        public string UserId
        {
            get => _userId;
            set => Set(ref _userId, value);
        }

        public Action SignOutAction { get; set; }

        public CatalogVoiceCommand CatalogVoiceCommand { get; set; }

        public ICommand SignInCommand => new RelayCommand(SignIn);

        public ICommand SignOutCommand => new RelayCommand(SignOut);

        public override void OnActivate(object parameter, bool isBack)
        {
            base.OnActivate(parameter, isBack);

            UserId = ApplicationData.Current.LocalSettings.Values[Constants.HelloUserIdKey] as string ?? string.Empty;
            _publicKeyHint = ApplicationData.Current.LocalSettings.Values[Constants.HelloPublicKeyHintKey] as string ?? string.Empty;

            StatusAuthenticationFeedbackToUser(KeyCredentialStatus.SecurityDeviceLocked);
        }

        public void StatusAuthenticationFeedbackToUser(KeyCredentialStatus setUp)
        {
            GreetingCustom = setUp.Equals(KeyCredentialStatus.Success) ? 
                string.Format(Constants.HelloSuccessCustomKey.GetLocalized(), UserId) : UserId;
            WaitingSignIn = setUp.Equals(KeyCredentialStatus.Success) ?
                Constants.HelloWelcomeKey.GetLocalized() : Constants.HelloMakingSureKey.GetLocalized();

            RaisePropertyChanged(() => GreetingCustom);
            RaisePropertyChanged(() => WaitingSignIn);
        }

        private async void SignIn()
        {
            // Open the existing user key credential.
            var retrieveResult = await KeyCredentialManager.OpenAsync(UserId);
            if (retrieveResult.Status != KeyCredentialStatus.Success) return;

            var userCredential = retrieveResult.Credential;

            // Sign the challenge using the user's KeyCredential.
            var challengeBuffer = CryptographicBuffer.DecodeFromBase64String("challenge");
            var opResult = await userCredential.RequestSignAsync(challengeBuffer);

            if (opResult.Status != KeyCredentialStatus.Success) return;

            StatusAuthenticationFeedbackToUser(opResult.Status);

            ShellStartup.Start(CatalogVoiceCommand);
            CatalogVoiceCommand = null;
        }

        private async void SignOut()
        {
            try
            {
                await KeyCredentialManager.DeleteAsync(UserId);
            }
            catch (Exception)
            {
                //Ignore
            }

            // Remove our app's knowledge of the user.
            ApplicationData.Current.LocalSettings.Values.Remove(Constants.HelloPublicKeyHintKey);

            SignOutAction?.Invoke();
        }
    }
}
