using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using eShop.Domain.Models;
using eShop.Providers.Contracts;
using eShop.UWP.Helpers;
using eShop.UWP.ViewModels.Base;
using GalaSoft.MvvmLight.Command;

namespace eShop.UWP.ViewModels.Catalog
{
    public class CatalogViewModel : CustomViewModelBase
    {
        private readonly ICatalogProvider _catalogProvider;
        private readonly ItemsGridViewModel _itemsGridViewModel;
        private readonly ItemsListViewModel _itemsListViewModel;

        private string _query;
        private CatalogType _selectedCatalogType;
        private CatalogBrand _selectedCatalogBrand;
        private ItemsContainViewModel _catalogFormat;
        private List<CatalogType> _catalogTypes;
        private List<CatalogBrand> _catalogBrands;
        private Dictionary<int, bool> _filterValues = new Dictionary<int, bool>();

        public CatalogViewModel(ICatalogProvider catalogProvider, ItemsGridViewModel itemsGridViewModel, ItemsListViewModel itemsListViewModel)
        {
            _catalogProvider = catalogProvider;
            _itemsGridViewModel = itemsGridViewModel;
            _itemsListViewModel = itemsListViewModel;

            ShowGrid();
        }

        public List<CatalogType> CatalogTypes
        {
            get => _catalogTypes;
            private set => Set(ref _catalogTypes, value);
        }

        public List<CatalogBrand> CatalogBrands
        {
            get => _catalogBrands;
            private set => Set(ref _catalogBrands, value);
        }

        public CatalogType SelectedCatalogType
        {
            get => _selectedCatalogType;
            set
            {
                Set(ref _selectedCatalogType, value);
                LoadCatalogItems();
            }
        }

        public CatalogBrand SelectedCatalogBrand
        {
            get => _selectedCatalogBrand;
            set
            {
                Set(ref _selectedCatalogBrand, value);
                LoadCatalogItems();
            }
        }

        public ItemsContainViewModel CatalogFormat
        {
            get => _catalogFormat;
            private set
            {
                Set(ref _catalogFormat, value);
                RaisePropertyChanged(() => IsGridFormatChecked);
            }
        }

        public bool IsGridFormatChecked
        {
            get => CatalogFormat == null || CatalogFormat == _itemsGridViewModel;
        }

        public ICommand ShowGridCommand => new RelayCommand(ShowGrid);

        public ICommand ShowListCommand => new RelayCommand(ShowList);

        public override void OnActivate(object parameter, bool isBack)
        {
            base.OnActivate(parameter, isBack);

            if (isBack) return;

            if (CatalogTypes == null)
            {
                LoadCatalogTypes();
            }

            if (CatalogBrands == null)
            {
                LoadCatalogBrands();
            }

            ResetCatalog();
        }

        public void ResetCatalog()
        {
            SelectedCatalogType = CatalogTypes.FirstOrDefault();
            SelectedCatalogBrand = CatalogBrands.FirstOrDefault();
            LoadCatalogItems();
        }

        public void Search(string query)
        {
            _query = query;
            LoadCatalogItems();
        }

        public void ShowGrid()
        {
            ActivateItemsContain(_itemsGridViewModel);
        }

        public void ShowList()
        {
            ActivateItemsContain(_itemsListViewModel);
        }

        public void ActivateItemsContain(ItemsContainViewModel itemsContain)
        {
            CatalogFormat = itemsContain;
            LoadCatalogItems();
        }

        private void LoadCatalogItems()
        {
            if (_selectedCatalogType == null || _selectedCatalogBrand == null) return;
            CatalogFormat.LoadCatalogItems(_selectedCatalogType, _selectedCatalogBrand, _query);
        }

        private void LoadCatalogTypes()
        {
            var types = _catalogProvider.GetCatalogTypes().ToList();
            var viewAll = new CatalogType { Id = 0, Type = Constants.CatalogAllViewKey.GetLocalized() };

            types.Insert(0, viewAll);
            CatalogTypes = types;
        }

        private void LoadCatalogBrands()
        {
            var types = _catalogProvider.GetCatalogBrands().ToList();
            var viewAll = new CatalogBrand { Id = 0, Brand = Constants.CatalogAllViewKey.GetLocalized() };

            types.Insert(0, viewAll);
            CatalogBrands = types;
        }
    }
}
