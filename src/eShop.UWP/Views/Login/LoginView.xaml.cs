using System;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using eShop.UWP.ViewModels;
using eShop.UWP.Activation;
using eShop.UWP.Animations;

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
            CurrentEffectMode = EffectMode.None;

            var activationState = e.Parameter as ActivationState;
            activationState = activationState ?? ActivationState.Default;

            await ViewModel.InitializeAsync(activationState);
        }

        protected override async void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                DoEffectOut();
                await Task.Delay(100);
                ViewModel.Login();
            }
            base.OnKeyDown(e);
        }

        private async void OnShowLoginWithPassword(object sender, RoutedEventArgs e)
        {
            await Task.Delay(100);
            passwordView.Focus();
        }

        static public EffectMode CurrentEffectMode = EffectMode.None;

        private void OnBackgroundFocus(object sender, RoutedEventArgs e)
        {
            DoEffectIn();
        }

        private void OnForegroundFocus(object sender, RoutedEventArgs e)
        {
            DoEffectOut();
        }

        private void DoEffectIn(double milliseconds = 1000)
        {
            if (CurrentEffectMode == EffectMode.Foreground || CurrentEffectMode == EffectMode.None)
            {
                CurrentEffectMode = EffectMode.Background;
                background.Scale(milliseconds, 1.0, 1.1);
                background.Blur(milliseconds, 6.0, 0.0);
                foreground.Scale(500, 1.0, 0.95);
                foreground.Fade(milliseconds, 1.0, 0.75);
            }
        }

        private void DoEffectOut(double milliseconds = 1000)
        {
            if (CurrentEffectMode == EffectMode.Background || CurrentEffectMode == EffectMode.None)
            {
                CurrentEffectMode = EffectMode.Foreground;
                background.Scale(milliseconds, 1.1, 1.0);
                background.Blur(milliseconds, 0.0, 6.0);
                foreground.Scale(500, 0.95, 1.0);
                foreground.Fade(milliseconds, 0.75, 1.0);
            }
        }

        public enum EffectMode
        {
            None,
            Background,
            Foreground,
            Disabled
        }
    }
}
