using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using eShop.UWP.ViewModels;
using eShop.UWP.Activation;

namespace eShop.UWP.Views
{
    public sealed partial class LoginView : Page
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private LoginViewModel ViewModel => DataContext as LoginViewModel;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var activationState = e.Parameter as ActivationState;
            activationState = activationState ?? ActivationState.Default;

            await ViewModel.InitializeAsync(activationState);
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                ViewModel.Login();
            }
            base.OnKeyDown(e);
        }
    }
}
