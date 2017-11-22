using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Composition;

using Microsoft.Graphics.Canvas.Effects;

namespace eShop.UWP.Controls
{
    public sealed class GrayscaleBrush : XamlCompositionBrushBase
    {
        protected override void OnConnected()
        {
            // Delay creating composition resources until they're required.
            if (CompositionBrush == null)
            {
                var backdrop = Window.Current.Compositor.CreateBackdropBrush();

                // Use a Win2D effect applied to a CompositionBackdropBrush.
                var effect = new GrayscaleEffect
                {
                    Name = "Grayscale",
                    Source = new OpacityEffect
                    {
                        Name = "GrayscaleOpacity",
                        Opacity = 1.0f,
                        Source = new CompositionEffectSourceParameter("backdrop")
                    }
                };

                var effectFactory = Window.Current.Compositor.CreateEffectFactory(effect);
                var effectBrush = effectFactory.CreateBrush();
                effectBrush.SetSourceParameter("backdrop", backdrop);

                CompositionBrush = effectBrush;
            }
        }

        protected override void OnDisconnected()
        {
            // Dispose of composition resources when no longer in use.
            if (CompositionBrush != null)
            {
                CompositionBrush.Dispose();
                CompositionBrush = null;
            }
        }
    }
}
