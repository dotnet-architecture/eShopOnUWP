using System;

using Windows.UI.Xaml.Controls;

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

        public void Initialize()
        {
            ViewModel.ItemsControl = gridView;
        }
    }
}
