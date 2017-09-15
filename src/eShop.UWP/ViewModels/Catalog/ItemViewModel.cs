using System;
using System.Collections.Generic;
using System.Windows.Input;
using eShop.Domain.Models;
using eShop.UWP.Helpers;
using eShop.UWP.ViewModels.Base;
using GalaSoft.MvvmLight.Command;

namespace eShop.UWP.ViewModels.Catalog
{
    public class ItemViewModel : CustomViewModelBase
    {
        private readonly Action<ItemViewModel, bool> _deleteAction;

        private List<bool> _catalogStates = new List<bool> { true, false };

        public ItemViewModel(CatalogItem item, Action<ItemViewModel, bool> deleteAction)
        {
            Item = item;
            _deleteAction = deleteAction;
        }

        public string PictureUri
        {
            get => Item.PictureUri;
            set
            {
                Item.PictureUri = value;
                RaisePropertyChanged(() => PictureUri);
            }
        }

        public string Name
        {
            get => Item.Name;
            set
            {
                Item.Name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public string CatalogTypeName => Item.CatalogType?.Type;

        public CatalogType CatalogType
        {
            get => Item.CatalogType;
            set
            {
                Item.CatalogType = value;
                RaisePropertyChanged(() => CatalogType);
                RaisePropertyChanged(() => CatalogTypeName);
            }
        }

        public string CatalogBrandName => Item.CatalogBrand?.Brand;

        public CatalogBrand CatalogBrand
        {
            get => Item.CatalogBrand;
            set
            {
                Item.CatalogBrand = value;
                RaisePropertyChanged(() => CatalogBrand);
                RaisePropertyChanged(() => CatalogBrandName);
            }
        }

        public string CatalogStateName => Item.IsActive ? Constants.ActivateStateKey.GetLocalized() : Constants.InactiveStateKey.GetLocalized();

        public bool? CatalogState
        {
            get => Item.IsActive;
            set
            {
                if (value == null) return;
                Item.IsActive = value.Value;
                RaisePropertyChanged(() => CatalogState);
                RaisePropertyChanged(() => CatalogStateName);
            }
        }

        public List<bool> CatalogStates
        {
            get => _catalogStates;
            set => Set(ref _catalogStates, value);
        }

        public double Price
        {
            get => Item.Price;
            set 
            {
                Item.Price = value;
                RaisePropertyChanged(() => Price);
            }
        }

        public CatalogItem Item { private set; get; }

        public ICommand DeleteCommand => new RelayCommand(Delete);


        public void Delete()
        {
            _deleteAction?.Invoke(this, false);
        }

        public void Edit()
        {
            NavigationService.Navigate(typeof(ItemDetailViewModel).FullName, Item);
        }

        public void SwitchState()
        {
            CatalogState = !CatalogState;
        }
    }
}
