using System;

using Windows.UI.Xaml.Controls;

namespace eShop.UWP
{
    public interface IShell
    {
        Frame NavigationFrame { get; }
        NavigationViewItem SettingsItem { get; }
    }
}
