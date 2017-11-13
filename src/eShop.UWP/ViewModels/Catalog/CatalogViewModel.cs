using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using eShop.UWP.Models;
using eShop.Providers;

namespace eShop.UWP.ViewModels
{
    public class CatalogViewModel : CommonViewModel
    {
        public CatalogViewModel()
        {
        }

        public CatalogState State { get; private set; }

        public IList<CatalogTypeModel> CatalogTypes { get; set; }
        public IList<CatalogBrandModel> CatalogBrands { get; set; }

        public ItemsGridViewModel GridViewModel { get; set; }
        public ItemsListViewModel ListViewModel { get; set; }

        private bool _isGridChecked = false;
        public bool IsGridChecked
        {
            get { return _isGridChecked; }
            set { Set(ref _isGridChecked, value); ViewSelectionChanged(); }
        }

        private bool _isListChecked = false;
        public bool IsListChecked
        {
            get { return _isListChecked; }
            set { Set(ref _isListChecked, value); ViewSelectionChanged(); }
        }

        public async Task LoadAsync(CatalogState state)
        {
            State = state;

            var provider = new CatalogProvider();

            CatalogTypes = await provider.GetCatalogTypesAsync();
            CatalogBrands = await provider.GetCatalogBrandsAsync();

            var items = await provider.GetItemsAsync(-1, -1, State.Query);
            var collectionItems = new ObservableCollection<CatalogItemModel>(items);
            GridViewModel.Items = collectionItems;
            ListViewModel.Items = collectionItems;

            IsGridChecked = State.IsGridChecked;
            IsListChecked = State.IsListChecked;
            GridViewModel.Mode = GridCommandBarMode.Idle;

            GridViewModel.UpdateCommandBar();

            HeaderText = State.Query == null ? "Catalog" : $"Catalog results for \"{State.Query}\"";
        }

        public async Task UnloadAsync()
        {
            State.IsGridChecked = IsGridChecked;
            State.IsListChecked = IsListChecked;
            if (GridViewModel.Items != null)
            {
                foreach (var item in GridViewModel.Items.Where(r => r.HasChanges))
                {
                    var provider = new CatalogProvider();
                    item.Commit();
                    await provider.SaveItemAsync(item);
                }
            }
        }

        private void ViewSelectionChanged()
        {
            GridViewModel.IsCommandBarOpen = false;
            ListViewModel.IsCommandBarOpen = false;

            GridViewModel.IsActive = _isGridChecked && !_isListChecked;
            ListViewModel.IsActive = _isListChecked && !_isGridChecked;

            if (GridViewModel.IsActive)
            {
                GridViewModel.UpdateExternalSelection();
            }

            if (ListViewModel.IsActive)
            {
                ListViewModel.UpdateExternalSelection();
            }
        }
    }
}
