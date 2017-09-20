using eShop.UWP.ViewModels.Catalog;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace eShop.UWP.Views.Catalog
{
    public sealed partial class ItemsGridView : UserControl
    {
        public ItemsGridViewModel ViewModel => DataContext as ItemsGridViewModel;

        public ItemsGridView()
        {
            InitializeComponent();
        }

        private void OnCatalogItemEditClick(object sender, System.EventArgs e)
        {
            ViewModel?.ShowDetail(sender as ItemViewModel, AdaptiveGrid);
        }

        private void OnSelectItemClick(object sender, RoutedEventArgs e)
        {
            AdaptiveGrid.SelectedItems.Add((sender as FrameworkElement)?.DataContext);
        }

        private void SelectAll_OnClick(object sender, RoutedEventArgs e)
        {
            AdaptiveGrid.SelectAll();
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            AdaptiveGrid.DeselectRange(new ItemIndexRange(0, (uint)AdaptiveGrid.Items.Count));
        }
    }
}
