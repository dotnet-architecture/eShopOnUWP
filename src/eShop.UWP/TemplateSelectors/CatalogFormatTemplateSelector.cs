using eShop.UWP.ViewModels.Catalog;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace eShop.UWP.TemplateSelectors
{
    public class CatalogFormatTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ItemsGridTemplate { get; set; }
        public DataTemplate ItemsListTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is ItemsGridViewModel)
            {
                return ItemsGridTemplate;
            }
            return ItemsListTemplate;
        }
    }
}
