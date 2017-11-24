using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using eShop.UWP.ViewModels;
using eShop.UWP.Activation;

namespace eShop.UWP.Views
{
    public sealed partial class ShellView : Page, IShell
    {
        public ShellView()
        {
            InitializeComponent();
        }

        private ShellViewModel ViewModel => DataContext as ShellViewModel;

        public Frame NavigationFrame => shellFrame;

        public NavigationViewItem SettingsItem => navigationView.SettingsItem as NavigationViewItem;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var activationState = e.Parameter as ActivationState;
            activationState = activationState ?? ActivationState.Default;
            ViewModel.Initialize(this, activationState);
        }
    }
}
