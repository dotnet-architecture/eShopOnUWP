using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShop.Domain.Models;
using eShop.Providers.Contracts;
using eShop.UWP.Helpers;
using Telerik.UI.Xaml.Controls.Grid;
using Windows.UI.Xaml.Controls;

namespace eShop.UWP.ViewModels.Catalog
{
    public class ItemsListViewModel : ItemsContainViewModel
    {
        private List<CatalogBrand> _catalogBrands;
        private List<CatalogType> _catalogTypes;

        public ItemsListViewModel(ICatalogProvider catalogProvider) : base(catalogProvider)
        {
            LoadCatalogBrands();
            LoadCatalogTypes();
        }

        public List<CatalogType> CatalogTypes
        {
            get => _catalogTypes;
            set => Set(ref _catalogTypes, value);
        }

        public List<CatalogBrand> CatalogBrands
        {
            get => _catalogBrands;
            set => Set(ref _catalogBrands, value);
        }

        public override async Task DeleteSelection(object control)
        {
            var radDataGrid = control as RadDataGrid;
            if (radDataGrid == null) return;
            var selectedItems = radDataGrid.SelectedItems.Cast<ItemViewModel>().ToList();

            var result = await ShowNotification(selectedItems);
            if (result != ContentDialogResult.Secondary) return;

            selectedItems.ForEach(item => DeleteItem(item, true));
            radDataGrid.SelectedItem = null;
        }

        public void ShowDetail(ItemViewModel item)
        {
            if (item == null) return;

            item.Edit();
            LastSelectedItem = item;
        }

        public void SelectionChanged(RadDataGrid radGridView)
        {
            SelectedItemsCount = radGridView.SelectedItems.Count == 1
                ? Constants.CommandBarCoutItemKey.GetLocalized()
                : string.Format(Constants.CommandBarCoutItemsKey.GetLocalized(), radGridView.SelectedItems.Count);
            IsMultiselectionEnable = radGridView.SelectedItems.Any();
        }

        private void LoadCatalogBrands()
        {
            if (CatalogBrands != null) return;
            
            CatalogBrands = CatalogProvider.GetCatalogBrands().ToList();
        }

        private void LoadCatalogTypes()
        {
            if (CatalogTypes != null) return;
            
            CatalogTypes = CatalogProvider.GetCatalogTypes().ToList();
        }
    }
}
