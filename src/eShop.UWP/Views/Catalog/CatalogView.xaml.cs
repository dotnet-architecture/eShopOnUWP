using System;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using eShop.UWP.ViewModels;

namespace eShop.UWP.Views
{
    public sealed partial class CatalogView : Page
    {
        public CatalogView()
        {
            InitializeComponent();
        }

        public CatalogViewModel ViewModel => DataContext as CatalogViewModel;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel.GridViewModel = new ItemsGridViewModel();
            ViewModel.ListViewModel = new ItemsListViewModel();

            itemsGrid.Initialize(ViewModel.GridViewModel);
            itemsList.Initialize(ViewModel.ListViewModel);

            var state = (e.Parameter as CatalogState) ?? new CatalogState();
            await ViewModel.LoadAsync(state);

            itemsList.CatalogTypes = ViewModel.CatalogTypes;
            itemsList.CatalogBrands = ViewModel.CatalogBrands;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            ViewModel.Unload();
        }
    }
}
