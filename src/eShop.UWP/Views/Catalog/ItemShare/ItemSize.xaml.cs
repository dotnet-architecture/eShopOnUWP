using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using eShop.UWP.Models;

namespace eShop.UWP.Views
{
    public sealed partial class ItemSize : UserControl
    {
        public ItemSize()
        {
            InitializeComponent();
        }

        public CatalogItemModel Model { get; set; }
    }
}
