using System;
using System.Collections.Generic;

using Windows.UI.Xaml.Controls;

using eShop.UWP.Models;
using eShop.UWP.ViewModels;

namespace eShop.UWP.Views
{
    public sealed partial class ItemsListView : UserControl
    {
        public ItemsListView()
        {
            this.InitializeComponent();
        }

        public ItemsListViewModel ViewModel => DataContext as ItemsListViewModel;

        public IList<CatalogTypeModel> CatalogTypes { get; set; }
        public IList<CatalogBrandModel> CatalogBrands { get; set; }

        public void Initialize(ItemsListViewModel viewModel)
        {
            DataContext = viewModel;
            ViewModel.ItemsControl = gridView;
        }
    }
}
