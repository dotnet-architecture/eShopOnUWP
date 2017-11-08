using System;

using Windows.UI.Xaml.Controls;

using GalaSoft.MvvmLight;

namespace eShop.UWP.Models
{
    public class NavigationItemModel : ObservableObject
    {
        public NavigationItemModel(Symbol symbol, string label, string key)
        {
            Icon = new SymbolIcon(symbol);
            Content = label;
            Key = key;
            Execute = null;
        }

        public IconElement Icon { get; set; }
        public object Content { get; set; }

        public string Key { get; set; }

        public Action<NavigationItemModel> Execute { get; set; }

        public override string ToString()
        {
            return $"{Content}";
        }
    }
}
