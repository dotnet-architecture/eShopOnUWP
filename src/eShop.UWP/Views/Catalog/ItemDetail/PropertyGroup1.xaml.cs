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
        }

        public ItemDetailViewModel ViewModel { get; set; }

        public void UpdateBindings()
        {
            Bindings.Update();
        }
    }
}
