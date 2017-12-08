using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using eShop.UWP.ViewModels;

namespace eShop.UWP.Views
{
    public sealed partial class TabSize : UserControl
    {
        public TabSize()
        {
            this.InitializeComponent();
        }

        public ItemDetailViewModel ViewModel { get; set; }
    }
}
