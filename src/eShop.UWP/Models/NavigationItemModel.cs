using System;

using Windows.UI.Xaml.Controls;

using GalaSoft.MvvmLight;

namespace eShop.UWP.Models
{
    public class NavigationItemModel : ObservableObject
    {
        public NavigationItemModel(string label, string key)
        {
            Content = label;
            Key = key;
            Execute = null;
        }
        public NavigationItemModel(Symbol symbol, string label, string key) : this(label, key)
        {
            Icon = new SymbolIcon(symbol);
        }
        public NavigationItemModel(int glyph, string label, string key) : this(label, key)
        {
            Icon = new FontIcon { Glyph = Char.ConvertFromUtf32(glyph).ToString() };
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
