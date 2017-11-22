using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using eShop.UWP.ViewModels;

namespace eShop.UWP.Views
{
    public sealed partial class ShellView : Page, IShell
    {
        public ShellView()
        {
            InitializeComponent();
            ViewModel.Initialize(this);
        }

        public Frame NavigationFrame => shellFrame;

        public NavigationViewItem SettingsItem => navigationView.SettingsItem as NavigationViewItem;

        private ShellViewModel ViewModel => DataContext as ShellViewModel;

        static public void Startup()
        {
            var shell = new ShellView();
            Window.Current.Content = shell;
            shell.NavigationFrame.Navigate(typeof(CatalogView), new CatalogState());
        }
    }
}
