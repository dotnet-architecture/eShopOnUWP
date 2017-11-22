using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using eShop.UWP.ViewModels;

namespace eShop.UWP.Views
{
    public sealed partial class LoginView : Page
    {
        public LoginView()
        {
            this.InitializeComponent();
            ViewModel.Initialize();
        }

        private LoginViewModel ViewModel => DataContext as LoginViewModel;

        static public void Startup()
        {
            var login = new LoginView();
            Window.Current.Content = login;
        }
    }
}
