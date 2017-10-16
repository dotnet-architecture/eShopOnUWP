using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using eShop.Domain.Models;
using eShop.Providers.Contracts;
using eShop.UWP.Helpers;
using eShop.UWP.ViewModels.Base;
using GalaSoft.MvvmLight.Command;
using Windows.UI.Xaml.Controls;
using eShop.UWP.Services;

namespace eShop.UWP.ViewModels.Catalog
{
    public class ItemsContainViewModel : CustomViewModelBase
    {
        protected readonly ICatalogProvider CatalogProvider;

        private ObservableCollection<ItemViewModel> _items;
        private bool _isMultiselectionEnable;
        private string _selectedItemsCount;

        public ItemsContainViewModel(ICatalogProvider catalogProvider)
        {
            CatalogProvider = catalogProvider;
        }

        public bool IsMultiselectionEnable
        {
            get => _isMultiselectionEnable;
            set => Set(ref _isMultiselectionEnable, value);
        }

        public string SelectedItemsCount
        {
            get => _selectedItemsCount;
            set => Set(ref _selectedItemsCount, value);
        }

        public ObservableCollection<ItemViewModel> Items
        {
            get => _items;
            set => Set(ref _items, value);
        }

        public ItemViewModel LastSelectedItem { get; set; }

        public ICommand DeleteSelectionCommand => new RelayCommand<object>(async control => await DeleteSelection(control));


        public virtual async Task DeleteSelection(object control)
        {
            await Task.Yield();
        }

        public async void DeleteItem(ItemViewModel itemViewModel, bool forceDelete = false)
        {
            if (!forceDelete)
            {
                var dialog = new ContentDialog
                {
                    Title = Constants.ConfirmationDialogDeleteItemTitleKey.GetLocalized(),
                    Content = Constants.ConfirmationDialogDeleteItemContentKey.GetLocalized(),
                    PrimaryButtonText = Constants.ConfirmationDialogCancelKey.GetLocalized(),
                    SecondaryButtonText = Constants.ConfirmationDialogDeleteKey.GetLocalized()
                };

                var result = await dialog.ShowAsync();
                if (result != ContentDialogResult.Secondary) return;
            }

            await CatalogProvider.DeleteItemAsync(itemViewModel.Item);
            Items.Remove(itemViewModel);

            if (!forceDelete)
            {
                Singleton<ToastNotificationsService>.Instance.ShowToastNotification(Constants.NotificationDeletedItemTitleKey.GetLocalized(), itemViewModel.Item);
            }
        }

        public async void LoadCatalogItems(CatalogType selectedCatalogType, CatalogBrand selectedCatalogBrand, string query)
        {
            IsMultiselectionEnable = false;
            var items = await CatalogProvider?.GetItemsAsync(selectedCatalogType, selectedCatalogBrand, query);
            Items = new ObservableCollection<ItemViewModel>(items.Select(item => new ItemViewModel(item, DeleteItem)));
        }

        protected async Task<ContentDialogResult> ShowNotification(List<ItemViewModel> selectedItems)
        {
            var dialog = new ContentDialog
            {
                Title = selectedItems.Count > 1
                    ? Constants.ConfirmationDialogDeleteItemsTitleKey.GetLocalized()
                    : Constants.ConfirmationDialogDeleteItemTitleKey.GetLocalized(),
                Content = selectedItems.Count > 1
                    ? Constants.ConfirmationDialogDeleteItemsContentKey.GetLocalized()
                    : Constants.ConfirmationDialogDeleteItemContentKey.GetLocalized(),
                PrimaryButtonText = Constants.ConfirmationDialogCancelKey.GetLocalized(),
                SecondaryButtonText = Constants.ConfirmationDialogDeleteKey.GetLocalized()
            };

            var result = await dialog.ShowAsync();
            return result;
        }
    }
}
