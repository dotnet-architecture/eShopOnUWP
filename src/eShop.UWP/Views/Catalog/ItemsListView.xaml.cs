using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShop.UWP.Helpers;
using eShop.UWP.ViewModels.Catalog;
using Telerik.UI.Xaml.Controls.Grid;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace eShop.UWP.Views.Catalog
{
    public sealed partial class ItemsListView : UserControl
    {
        public ItemsListViewModel ViewModel => DataContext as ItemsListViewModel;

        public ItemsListView()
        {
            InitializeComponent();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel?.SelectionChanged(DataGrid);

            var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation(Constants.ConnectedAnimationKey);
            animation?.Cancel();

            var lastSelectedItem = ViewModel?.LastSelectedItem;
            var items = ViewModel?.Items;
            var item = items?.FirstOrDefault(i => i.Item.Id == lastSelectedItem?.Item.Id);

            if (item == null) return;

            await Task.Delay(100);
            DataGrid.ScrollItemIntoView(item);
            DataGrid.UpdateLayout();
        }

        private void OnEditClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var selectedItemViewModel = button.DataContext as ItemViewModel;
            var images = new List<Image>();
            DataGrid.FindChildren(images);

            var selectedImage = images.FirstOrDefault(image => (image.DataContext as ItemViewModel)?.Item == selectedItemViewModel?.Item);
            if (selectedImage == null) return;

            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate(Constants.ConnectedAnimationKey, selectedImage);

            ViewModel?.ShowDetail(selectedItemViewModel);
        }

        private void OnSelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {
            if (ViewModel == null) return;
            var radDataGrid = (RadDataGrid)sender;

            ViewModel?.SelectionChanged(radDataGrid);
        }

        private void OnSelectAllClick(object sender, RoutedEventArgs e)
        {
            DataGrid.SelectAll();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DataGrid.SelectedItem = null;
        }
    }
}
