using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace eShop.UWP
{
    public static class FrameExtensions
    {
        public static TResult TryGetResource<TResult>(this Frame frame, string key, TResult defaultValue = default(TResult))
        {
            if (frame.Content is FrameworkElement ui)
            {
                if (ui.Resources.ContainsKey(key))
                {
                    return (TResult)ui.Resources[key];
                }
            }
            return defaultValue;
        }
    }
}
