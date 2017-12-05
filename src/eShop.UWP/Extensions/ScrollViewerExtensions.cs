using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace eShop.UWP.Animations
{
    static public class ScrollViewerExtensions
    {
        static public void StopElementAtOffset(this ScrollViewer scrollViewer, UIElement element, float offset)
        {
            var compositor = Window.Current.Compositor;
            var propertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);

            ElementCompositionPreview.SetIsTranslationEnabled(element, true);

            compositor.CreateExpressionWrapper("Max(0, -scroller.Translation.Y-offset)")
                .Parameter("scroller", propertySet)
                .Parameter("offset", offset)
                .Start(element, "Translation.Y");
        }
    }
}
