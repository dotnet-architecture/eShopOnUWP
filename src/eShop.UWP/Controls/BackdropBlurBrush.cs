using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Composition;

using Microsoft.Graphics.Canvas.Effects;

namespace eShop.UWP.Controls
{
    public sealed class BackdropBlurBrush : XamlCompositionBrushBase
    {
        #region BlurAmount
        public double BlurAmount
        {
            get { return (double)GetValue(BlurAmountProperty); }
            set { SetValue(BlurAmountProperty, value); }
        }

        private static void BlurAmountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = d as BackdropBlurBrush;
            me.CompositionBrush?.Properties.InsertScalar("Blur.BlurAmount", (float)e.NewValue);
        }

        public static readonly DependencyProperty BlurAmountProperty = DependencyProperty.Register("BlurAmount", typeof(double), typeof(BackdropBlurBrush), new PropertyMetadata(0.0, BlurAmountChanged));
        #endregion

        #region BlurAmountExpression
        public ExpressionAnimation BlurAmountExpression
        {
            get { return (ExpressionAnimation)GetValue(BlurAmountExpressionProperty); }
            set { SetValue(BlurAmountExpressionProperty, value); }
        }

        private static void BlurAmountExpressionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = d as BackdropBlurBrush;
            var expression = e.NewValue as ExpressionAnimation;
            if (expression != null)
            {
                me.CompositionBrush?.StartAnimation("Blur.BlurAmount", expression);
            }
        }

        public static readonly DependencyProperty BlurAmountExpressionProperty = DependencyProperty.Register("BlurAmountExpression", typeof(ExpressionAnimation), typeof(BackdropBlurBrush), new PropertyMetadata(null, BlurAmountExpressionChanged));
        #endregion

        protected override void OnConnected()
        {
            // Delay creating composition resources until they're required.
            if (CompositionBrush == null)
            {
                var backdrop = Window.Current.Compositor.CreateBackdropBrush();

                // Use a Win2D blur affect applied to a CompositionBackdropBrush.
                var graphicsEffect = new GaussianBlurEffect
                {
                    Name = "Blur",
                    BlurAmount = (float)this.BlurAmount,
                    Source = new CompositionEffectSourceParameter("backdrop")
                };

                var effectFactory = Window.Current.Compositor.CreateEffectFactory(graphicsEffect, new[] { "Blur.BlurAmount" });
                var effectBrush = effectFactory.CreateBrush();
                effectBrush.SetSourceParameter("backdrop", backdrop);

                CompositionBrush = effectBrush;
                if (BlurAmountExpression != null)
                {
                    CompositionBrush.StartAnimation("Blur.BlurAmount", BlurAmountExpression);
                }
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
