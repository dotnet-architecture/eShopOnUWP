using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using GalaSoft.MvvmLight.Command;

using eShop.Providers;
using eShop.UWP.Models;
using eShop.UWP.Services;
using eShop.UWP.Helpers;

namespace eShop.UWP.ViewModels
{
    public class ItemDetailViewModel : CommonViewModel
    {
        public ItemDetailViewModel(ICatalogProvider catalogProvider)
        {
            DataProvider = catalogProvider;
        }

        public ICatalogProvider DataProvider { get; }

        public ItemDetailState State { get; private set; }

        private IList<CatalogTypeModel> _catalogTypes = null;
        public IList<CatalogTypeModel> CatalogTypes
        {
            get { return _catalogTypes; }
            set { Set(ref _catalogTypes, value); }
        }

        private IList<CatalogBrandModel> _catalogBrands = null;
        public IList<CatalogBrandModel> CatalogBrands
        {
            get { return _catalogBrands; }
            set { Set(ref _catalogBrands, value); }
        }

        public override bool AlwaysShowHeader => false;

        private CatalogItemModel _item;
        public CatalogItemModel Item
        {
            get { return _item; }
            set { Set(ref _item, value); }
        }

        private ObservableCollection<CatalogItemModel> _relatedItems = null;
        public ObservableCollection<CatalogItemModel> RelatedItems
        {
            get { return _relatedItems; }
            set { Set(ref _relatedItems, value); }
        }

        private bool _isUnavailable;
        public bool IsUnavailable
        {
            get { return _isUnavailable; }
            set { Set(ref _isUnavailable, value); }
        }

        private bool _isCommandBarOpen = false;
        public bool IsCommandBarOpen
        {
            get { return _isCommandBarOpen; }
            set { Set(ref _isCommandBarOpen, value); }
        }

        public bool IsSaveVisible => true;
        public bool IsSeparatorVisible => IsSaveVisible && IsDeleteVisible;
        public bool IsDeleteVisible => Item?.Id != 0;

        public ICommand SaveCommand => new RelayCommand(OnSave);
        public ICommand DeleteCommand => new RelayCommand(OnDelete);

        public ICommand SelectPictureCommand => new RelayCommand(OnSelectPicture);

        private async void OnSave()
        {
            var result = Validate();
            if (result.IsOk)
            {
                try
                {
                    bool isNew = Item.Id == 0;
                    Item.Commit();
                    await DataProvider.SaveItemAsync(Item);
                    NavigationService.GoBack();
                    if (isNew)
                    {
                        ToastNotificationsService.Current.ShowToastNotification(Constants.NotificationAddedItemTitleKey.GetLocalized(), Item);
                    }
                }
                catch (Exception ex)
                {
                    await DialogBox.ShowAsync("Error saving item", ex);
                }
            }
            else
            {
                await DialogBox.ShowAsync(result);
            }
        }

        private async void OnDelete()
        {
            if (await DialogBox.ShowAsync("Confirm Delete", "Are you sure you want to delete this item?", "Ok", "Cancel"))
            {
                try
                {
                    await DataProvider.DeleteItemAsync(Item);
                    NavigationService.GoBack();
                    ToastNotificationsService.Current.ShowToastNotification(Constants.NotificationDeletedItemTitleKey.GetLocalized(), Item);
                }
                catch (Exception ex)
                {
                    await DialogBox.ShowAsync("Error deleting item", ex);
                }
            }
        }

        private async void OnSelectPicture()
        {
            var result = await ImagePicker.OpenAsync();
            if (result != null)
            {
                Item.Picture = result.ImageBytes;
                Item.PictureFileName = result.FileName;
                Item.PictureUri = result.ImageUri;
                Item.PictureContentType = result.ContentType;
            }
        }

        private Result Validate()
        {
            if (String.IsNullOrEmpty(Item.Name))
            {
                return Result.Error("Validation error", "Name cannot be null.");
            }
            if (Item.CatalogType.Id < 1)
            {
                return Result.Error("Validation error", "Catalog type cannot be empty.");
            }
            if (Item.CatalogBrand.Id < 1)
            {
                return Result.Error("Validation error", "Catalog brand cannot be empty.");
            }
            if (!(Item.Price > 0))
            {
                return Result.Error("Validation error", "Price must be greater than zero.");
            }
            return Result.Ok();
        }

        public async Task LoadAsync(ItemDetailState state)
        {
            State = state;

            CatalogTypes = await DataProvider.GetCatalogTypesAsync();
            CatalogBrands = await DataProvider.GetCatalogBrandsAsync();

            int typeId = 0;

            if (state.Item != null)
            {
                var item = await DataProvider.GetItemByIdAsync(state.Item.Id);
                if (item == null)
                {
                    item = state.Item;
                    IsUnavailable = true;
                }
                typeId = item.CatalogType.Id;
                Item = item;
            }
            else
            {
                Item = new CatalogItemModel();
            }
            var relatedItems = await DataProvider.GetItemsAsync(typeId, -1, null);
            var relatedItemsSkipCurrent = relatedItems.Where(r => r.Id != Item.Id);
            RelatedItems = new ObservableCollection<CatalogItemModel>(relatedItemsSkipCurrent);
        }

        public Task UnloadAsync()
        {
            RelatedItems = null;
            return Task.CompletedTask;
        }
    }
}
