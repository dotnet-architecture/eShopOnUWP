using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using eShop.UWP.ViewModels;

namespace eShop.UWP.Views
{
    public sealed partial class PropertyGroup1 : UserControl
    {
        public PropertyGroup1()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        public ItemDetailViewModel ViewModel { get; set; }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Bindings.Update();
        }
    }
}
