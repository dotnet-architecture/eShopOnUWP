using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using eShop.Providers.Contracts;
using eShop.UWP.Helpers;
using GalaSoft.MvvmLight.Command;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Animation;

namespace eShop.UWP.ViewModels.Catalog
{
    public class ItemsGridViewModel : ItemsContainViewModel
    {
        public ItemsGridViewModel(ICatalogProvider catalogProvider) : base(catalogProvider)
        {
        }
        
        public ICommand ItemClickCommand => new RelayCommand<ItemClickEventArgs>(ShowDetail);

        public ICommand SelectionChangedCommand => new RelayCommand<AdaptiveGridView>(SelectionChanged);

        public ICommand LoadedCommand => new RelayCommand<AdaptiveGridView>(OnLoaded);

        public ICommand CancelSelectionCommand => new RelayCommand<AdaptiveGridView>(CancelSelection);

        public override async Task DeleteSelection(object control)
        {
            var adaptativeGridView = control as AdaptiveGridView;
            if (adaptativeGridView == null) return;
            var selectedItems = adaptativeGridView.SelectedItems.Cast<ItemViewModel>().ToList();

            var result = await ShowNotification(selectedItems);
            if (result != ContentDialogResult.Secondary) return;

            selectedItems.ForEach(item => DeleteItem(item, true));
        }

        public void ShowDetail(ItemViewModel itemViewModel, AdaptiveGridView grid)
        {
            SetAnimation(itemViewModel, grid);
        }

        private void ShowDetail(ItemClickEventArgs arg)
        {
            var itemViewModel = arg.ClickedItem as ItemViewModel;
            var grid = arg.OriginalSource as AdaptiveGridView;

            SetAnimation(itemViewModel, grid);
        }

        private void SetAnimation(ItemViewModel itemViewModel, AdaptiveGridView grid)
        {
            if (itemViewModel == null || grid == null) return;

            grid.PrepareConnectedAnimation(Constants.ConnectedAnimationKey, itemViewModel, "SourceImage");
            itemViewModel.Edit();
            LastSelectedItem = itemViewModel;
        }

        private void OnLoaded(AdaptiveGridView adaptiveGrid)
        {
            IsMultiselectionEnable = false;

            if (adaptiveGrid == null || LastSelectedItem == null) return;
        
            var selectedItem = Items.FirstOrDefault(item => item.Item.Id == LastSelectedItem.Item.Id);

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

        private void CancelSelection(AdaptiveGridView adaptativeGridView)
        {
            adaptativeGridView.DeselectRange(new ItemIndexRange(0, (uint)Items.Count));
        }
    }
}
