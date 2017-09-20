using eShop.Cortana;
using eShop.UWP.ViewModels.Base;
using Windows.Storage;

namespace eShop.UWP.ViewModels.Login
{
    public class LoginViewModel : CustomViewModelBase
    {
        private readonly SignInWithHelloViewModel _signInWithHelloViewModel;
        private readonly SignInWithPasswordViewModel _signInWithPasswordViewModel;

        private string _userId;
        private string _publicKeyHint;
        private CustomViewModelBase _loginMethod;

        public LoginViewModel(SignInWithHelloViewModel signInWithHelloViewModel, SignInWithPasswordViewModel signInWithPasswordViewModel)
        {
            _signInWithHelloViewModel = signInWithHelloViewModel;
            _signInWithPasswordViewModel = signInWithPasswordViewModel;
            _signInWithHelloViewModel.SignOutAction = SignOut;
        }

        public CustomViewModelBase LoginMethod
        {
            get => _loginMethod;
            set => Set(ref _loginMethod, value);
        }

        public override void OnActivate(object parameter, bool isBack)
        {
            base.OnActivate(parameter, isBack);

            CleanBackStack();
            _userId = ApplicationData.Current.LocalSettings.Values[Constants.HelloUserIdKey] as string ?? string.Empty;
            _publicKeyHint = ApplicationData.Current.LocalSettings.Values[Constants.HelloPublicKeyHintKey] as string ?? string.Empty;

            if (parameter != null && parameter is CatalogVoiceCommand)
            {
                if (parameter is CatalogVoiceCommand catalogVoiceCommand)
                {
                    _signInWithHelloViewModel.CatalogVoiceCommand = catalogVoiceCommand;
                    _signInWithPasswordViewModel.CatalogVoiceCommand = catalogVoiceCommand;
                }
            }

            if (!string.IsNullOrEmpty(_userId) && !string.IsNullOrEmpty(_publicKeyHint))
            {
                _signInWithHelloViewModel.OnActivate(parameter, isBack);
                LoginMethod = _signInWithHelloViewModel;
            }
            else
            {
                _signInWithPasswordViewModel.OnActivate(parameter, isBack);
                LoginMethod = _signInWithPasswordViewModel;
            }
        }

        public void SignOut()
        {
            LoginMethod = _signInWithPasswordViewModel;
        }
    }
}
