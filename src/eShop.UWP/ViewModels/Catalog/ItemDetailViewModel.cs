using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using eShop.Cortana;
using eShop.Domain.Models;
using eShop.Providers.Contracts;
using eShop.UWP.Helpers;
using eShop.UWP.ViewModels.Base;
using GalaSoft.MvvmLight.Command;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Animation;
using eShop.UWP.Services;

namespace eShop.UWP.ViewModels.Catalog
{
    public class ItemDetailViewModel : CustomViewModelBase
    {
        private readonly ICatalogProvider _catalogProvider;
        private readonly SystemNavigationManager _systemNavigationManager;

        private string _pictureUri;
        private string _name;
        private string _description;
        private string _selectedItemsCount;
        private double _price;
        private bool _isActive;
        private bool _isMultiselectionEnable;
        private CatalogItem _item;
        private CatalogType _selectedCatalogType;
        private CatalogBrand _catalogBrand;
        private List<bool> _catalogStates = new List<bool> { true, false };
        private List<CatalogBrand> _catalogBrands;
        private List<CatalogType> _catalogTypes;
        private List<CatalogItem> _relatedItems;
        private ObservableCollection<ItemViewModel> _itemsViewModel;

        public ItemDetailViewModel(ICatalogProvider catalogProvider)
        {
            _catalogProvider = catalogProvider;
            _systemNavigationManager = SystemNavigationManager.GetForCurrentView();
        }

        public ObservableCollection<ItemViewModel> ItemsViewModel
        {
            get => _itemsViewModel;
            set => Set(ref _itemsViewModel, value);
        }

        public CatalogItem Item
        {
            get => _item;
            set => Set(ref _item, value);
        }

        public string PictureUri
        {
            get => _pictureUri;
            set => Set(ref _pictureUri, value);
        }

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        public double Price
        {
            get => _price;
            set => Set(ref _price, value);
        }

        public CatalogType SelectedCatalogType
        {
            get => _selectedCatalogType;
            set
            {
                Set(ref _selectedCatalogType, value);
                LoadRelatedItems();
            }
        }

        public List<CatalogType> CatalogTypes
        {
            get => _catalogTypes;
            set => Set(ref _catalogTypes, value);
        }

        public CatalogBrand SelectedCatalogBrand
        {
            get => _catalogBrand;
            set => Set(ref _catalogBrand, value);
        }

        public List<CatalogBrand> CatalogBrands
        {
            get => _catalogBrands;
            set => Set(ref _catalogBrands, value);
        }

        public bool SelectedCatalogState
        {
            get => _isActive;
            set => Set(ref _isActive, value);
        }

        public List<bool> CatalogStates
        {
            get => _catalogStates;
            set => Set(ref _catalogStates, value);
        }

        public List<CatalogItem> RelatedItems
        {
            get => _relatedItems;
            set => Set(ref _relatedItems, value);
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

        public ItemViewModel LastSelectedItem { get; set; }

        public ICommand SetImageCommand => new RelayCommand(SetImage);

        public ICommand DeleteCommand => new RelayCommand(Delete);

        public ICommand SaveCommand => new RelayCommand(Save);

        public ICommand ItemClickCommand => new RelayCommand<ItemClickEventArgs>(ShowDetail);

        public ICommand LoadedCommand => new RelayCommand<AdaptiveGridView>(OnLoaded);

        public ICommand SelectionChangedCommand => new RelayCommand<AdaptiveGridView>(SelectionChanged);

        public ICommand CancelSelectionCommand => new RelayCommand<AdaptiveGridView>(CancelSelection);

        public ICommand DeleteSelectionCommand => new RelayCommand<AdaptiveGridView>(async (adaptiveGridView) => await DeleteSelection(adaptiveGridView));

        public ICommand SelectAllCommand => new RelayCommand<AdaptiveGridView>(SelectAll);

        public override async void OnActivate(object parameter, bool isBack)
        {
            base.OnActivate(parameter, isBack);

            if (parameter != null && parameter is CatalogVoiceCommand)
            {
                int.TryParse((parameter as CatalogVoiceCommand).Value, out int itemId);

                var itemSelected = await _catalogProvider.GetItemByIdAsync(itemId);
                SaveCatalogItem(itemSelected as CatalogItem ?? new CatalogItem());
            }
            else
            {
                SaveCatalogItem(parameter as CatalogItem ?? new CatalogItem());
            }

            LoadCatalogBrands();
            LoadCatalogTypes();
            LoadRelatedItems();
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();

            _systemNavigationManager.BackRequested -= OnBackRequested;
            _systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        public async void SetImage()
        {
            var pictureUri = await UploadImageHelper.UploadImageAsync();
            if (string.IsNullOrEmpty(pictureUri)) return;

            PictureUri = pictureUri;
        }

        public void ShowDetail(ItemViewModel itemViewModel, AdaptiveGridView grid)
        {
            SetAnimation(itemViewModel, grid);
        }

        public async void DeleteItem(ItemViewModel itemViewModel, bool forceDelete)
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

            await _catalogProvider.DeleteItemAsync(itemViewModel.Item);
            _itemsViewModel.Remove(itemViewModel);

            if (!forceDelete)
            {
                Singleton<ToastNotificationsService>.Instance.ShowToastNotification(Constants.NotificationDeletedItemTitleKey.GetLocalized(), itemViewModel.Item);
            }
        }

        public async void DeleteItem(ItemViewModel itemViewModel)
        {
            await _catalogProvider.DeleteItemAsync(itemViewModel.Item);
            _itemsViewModel.Remove(itemViewModel);
        }

        public async void Delete()
        {
            await _catalogProvider.DeleteItemAsync(_item);
            Singleton<ToastNotificationsService>.Instance.ShowToastNotification(Constants.NotificationDeletedItemTitleKey.GetLocalized(), _item);
            NavigationService.Navigate(typeof(CatalogViewModel).FullName);
        }

        public async void Save()
        {
            var itemId = _item.Id;

            _item.Name = Name;
            _item.PictureUri = PictureUri;
            _item.Price = Price;
            _item.Description = Description;
            _item.CatalogType = SelectedCatalogType ?? new CatalogType();
            _item.CatalogTypeId = SelectedCatalogType.Id;
            _item.CatalogBrand = SelectedCatalogBrand ?? new CatalogBrand();
            _item.CatalogBrandId = SelectedCatalogBrand.Id;
            _item.IsActive = SelectedCatalogState;

            await _catalogProvider.SaveItemAsync(_item);
            if (itemId == 0)
            {
                Singleton<ToastNotificationsService>.Instance.ShowToastNotification(Constants.NotificationAddedItemTitleKey.GetLocalized(), _item);
            }

            NavigationService.Navigate(typeof(CatalogViewModel).FullName);
        }

        private void CancelSelection(AdaptiveGridView adaptativeGridView)
        {
            adaptativeGridView.DeselectRange(new ItemIndexRange(0, (uint)ItemsViewModel.Count));
        }

        private async Task DeleteSelection(AdaptiveGridView adaptativeGridView)
        {
            var selectedItems = adaptativeGridView.SelectedItems.Cast<ItemViewModel>().ToList();

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
            if (result != ContentDialogResult.Secondary) return;

            selectedItems.ForEach(item => DeleteItem(item, true));
        }

        private void SelectAll(AdaptiveGridView adaptativeGridView)
        {
            adaptativeGridView.SelectAll();
        }

        private async void ShowDetail(ItemClickEventArgs arg)
        {
            var selectedId = new CatalogItem();

            if (arg.ClickedItem is ItemViewModel)
            {
                var itemSelected = arg.ClickedItem as ItemViewModel;
                var itemSelectedId = await _catalogProvider.GetItemByIdAsync(itemSelected.Item.Id);
                SaveCatalogItem(itemSelectedId as CatalogItem ?? new CatalogItem());
            }
            else
            {
                selectedId = arg.ClickedItem as CatalogItem;
                SaveCatalogItem(selectedId);
            }

            if (selectedId == null) return;

            var item = _itemsViewModel.FirstOrDefault(itemId => itemId.Item.Id == selectedId.Id);

            var itemViewModel = item as ItemViewModel;
            var grid = arg.OriginalSource as AdaptiveGridView;

            SetAnimation(itemViewModel, grid);

        }

        private async void SetAnimation(ItemViewModel itemViewModel, AdaptiveGridView grid)
        {
            if (itemViewModel == null) return;

            var itemSelectedId = await _catalogProvider.GetItemByIdAsync(itemViewModel.Item.Id);
            SaveCatalogItem(itemSelectedId as CatalogItem ?? new CatalogItem());

            LastSelectedItem = itemViewModel;
        }

        private void SaveCatalogItem(CatalogItem selectedItem)
        {
            Item = selectedItem;
            Name = selectedItem.Name;
            PictureUri = selectedItem.PictureUri;
            Price = selectedItem.Price;
            Description = selectedItem.Description;
            SelectedCatalogState = selectedItem.IsActive;
            SelectedCatalogBrand = selectedItem.CatalogBrand;
            SelectedCatalogType = selectedItem.CatalogType;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
            NavigationService.GoBack();
        }

        private async void LoadCatalogBrands()
        {
            if (CatalogBrands != null) return;

            CatalogBrands = (await _catalogProvider.GetCatalogBrandsAsync()).ToList();
            SelectedCatalogBrand = _item.CatalogBrand ?? CatalogBrands.FirstOrDefault();
        }

        private async void LoadCatalogTypes()
        {
            if (CatalogTypes != null) return;

            CatalogTypes = (await _catalogProvider.GetCatalogTypesAsync()).ToList();
            SelectedCatalogType = _item.CatalogType ?? CatalogTypes.FirstOrDefault();
        }

        private async void LoadRelatedItems()
        {
            if (SelectedCatalogType == null) return;

            var items = await _catalogProvider.RelatedItemsByTypeAsync(SelectedCatalogType.Id);
            RelatedItems = items.ToList();

            ItemsViewModel = new ObservableCollection<ItemViewModel>(items.Select(item => new ItemViewModel(item, DeleteItem)));
        }

        private void OnLoaded(AdaptiveGridView adaptiveGrid)
        {
            IsMultiselectionEnable = false;

            if (adaptiveGrid == null || LastSelectedItem == null) return;

            var selectedItem = _itemsViewModel.FirstOrDefault(item => item.Item.Id == LastSelectedItem.Item.Id);

            if (selectedItem == null) return;

            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation(Constants.ConnectedAnimationKey);
            if (animation != null)
            {
                adaptiveGrid.ScrollIntoView(selectedItem, ScrollIntoViewAlignment.Default);
                adaptiveGrid.UpdateLayout();

                var containerObject = adaptiveGrid.ContainerFromItem(selectedItem);

                if (containerObject is GridViewItem container)
                {
                    var root = (FrameworkElement)container.ContentTemplateRoot;
                    var image = (Image)root.FindName("SourceImage");
                    animation.TryStart(image);
                }
                else
                {
                    animation.Cancel();
                }
            }

            LastSelectedItem = null;
        }

        private void SelectionChanged(AdaptiveGridView adaptativeGridView)
        {
            SelectedItemsCount = adaptativeGridView.SelectedItems.Count == 1
                ? Constants.CommandBarCoutItemKey.GetLocalized()
                : string.Format(Constants.CommandBarCoutItemsKey.GetLocalized(), adaptativeGridView.SelectedItems.Count);
            IsMultiselectionEnable = adaptativeGridView.SelectedItems.Any();
        }
    }
}
