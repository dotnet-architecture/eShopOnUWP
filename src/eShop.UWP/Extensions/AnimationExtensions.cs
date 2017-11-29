using System;
using System.Numerics;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Composition;
using Microsoft.Graphics.Canvas.Effects;

namespace eShop.UWP.Animations
{
    static public class AnimationExtensions
    {
        static public void Fade(this UIElement element, double milliseconds, double start, double end)
        {
            element.StartAnimation(nameof(Visual.Opacity), CreateScalarAnimation(milliseconds, start, end));
        }

        static public void Scale(this FrameworkElement element, double milliseconds, double start, double end)
        {
            element.SetCenterPoint(element.ActualWidth / 2.0, element.ActualHeight / 2.0);
            element.StartAnimation(nameof(Visual.Scale), CreateScaleAnimation(milliseconds, start, end));
        }

        static public void Blur(this UIElement element, double milliseconds, double start, double end)
        {
            var brush = CreateBlurEffectBrush();
            element.SetBrush(brush);
            brush.StartAnimation("Blur.BlurAmount", CreateScalarAnimation(milliseconds, start, end));
        }

        static public void SetBrush(this UIElement element, CompositionBrush brush)
        {
            var spriteVisual = CreateSpriteVisual(element);
            spriteVisual.Brush = brush;
            ElementCompositionPreview.SetElementChildVisual(element, spriteVisual);
        }

        static public SpriteVisual CreateSpriteVisual(UIElement element)
        {
            return CreateSpriteVisual(ElementCompositionPreview.GetElementVisual(element));
        }
        static public SpriteVisual CreateSpriteVisual(Visual elementVisual)
        {
            var compositor = elementVisual.Compositor;
            var spriteVisual = compositor.CreateSpriteVisual();
            var expression = compositor.CreateExpressionAnimation();
            expression.Expression = "visual.Size";
            expression.SetReferenceParameter("visual", elementVisual);
            spriteVisual.StartAnimation(nameof(Visual.Size), expression);
            return spriteVisual;
        }

        static public void SetCenterPoint(this UIElement element, double x, double y)
        {
            var visual = ElementCompositionPreview.GetElementVisual(element);
            visual.CenterPoint = new Vector3((float)x, (float)y, 0);
        }

        static public void StartAnimation(this UIElement element, string propertyName, CompositionAnimation animation)
        {
            var visual = ElementCompositionPreview.GetElementVisual(element);
            visual.StartAnimation(propertyName, animation);
        }

        static public CompositionAnimation CreateScalarAnimation(double milliseconds, double start, double end)
        {
            var animation = Window.Current.Compositor.CreateScalarKeyFrameAnimation();
            animation.InsertKeyFrame(0.0f, (float)start);
            animation.InsertKeyFrame(1.0f, (float)end);
            animation.Duration = TimeSpan.FromMilliseconds(milliseconds);
            return animation;
        }

        static public CompositionAnimation CreateScaleAnimation(double milliseconds, double start, double end)
        {
            var animation = Window.Current.Compositor.CreateVector3KeyFrameAnimation();
            animation.InsertKeyFrame(0.0f, new Vector3((float)start, (float)start, 0));
            animation.InsertKeyFrame(1.0f, new Vector3((float)end, (float)end, 0));
            animation.Duration = TimeSpan.FromMilliseconds(milliseconds);
            return animation;
        }

        static public CompositionEffectBrush CreateBlurEffectBrush(double amount = 0.0)
        {
            var effect = new GaussianBlurEffect
            {
                Name = "Blur",
                BlurAmount = (float)amount,
                Source = new CompositionEffectSourceParameter("source")
            };

            var compositor = Window.Current.Compositor;
            var factory = compositor.CreateEffectFactory(effect, new[] { "Blur.BlurAmount" });
            var brush = factory.CreateBrush();
            brush.SetSourceParameter("source", compositor.CreateBackdropBrush());
            return brush;
        }
    }
}
