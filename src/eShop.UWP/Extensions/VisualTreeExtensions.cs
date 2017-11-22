using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace eShop.UWP
{
    static public class VisualTreeExtensions
    {
        public static T GetChildOfType<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        public static T GetParentOfType<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            var parent = VisualTreeHelper.GetParent(depObj);

            var result = (parent as T) ?? GetParentOfType<T>(parent);
            if (result != null) return result;
            return null;
        }
    }
}
