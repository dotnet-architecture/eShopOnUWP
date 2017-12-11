using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using eShop.UWP.ViewModels;

namespace eShop.UWP.Views
{
    public sealed partial class RelatedItems : UserControl
    {
        public RelatedItems()
        {
            this.InitializeComponent();
        }

        public ItemDetailViewModel ViewModel { get; set; }

        private void gridView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}
