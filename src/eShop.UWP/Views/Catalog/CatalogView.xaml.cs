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

            itemsGrid.Initialize();
            itemsList.Initialize();

            ViewModel.GridViewModel = itemsGrid.ViewModel;
            ViewModel.ListViewModel = itemsList.ViewModel;

            var state = (e.Parameter as CatalogState) ?? new CatalogState();
            await ViewModel.LoadAsync(state);
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            await ViewModel.UnloadAsync();
        }
    }
}
