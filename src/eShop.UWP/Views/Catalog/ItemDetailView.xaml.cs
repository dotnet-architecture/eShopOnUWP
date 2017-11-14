using System;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Hosting;

using eShop.UWP.ViewModels;
using Windows.UI.Xaml.Media.Animation;

namespace eShop.UWP.Views
{
    public sealed partial class ItemDetailView : Page
    {
        public ItemDetailView()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("ItemSelected");
            if (imageAnimation != null)
            {
                imageAnimation.TryStart(picture);
            }
        }

        public ItemDetailViewModel ViewModel => DataContext as ItemDetailViewModel;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var state = (e.Parameter as ItemDetailState) ?? new ItemDetailState();
            await ViewModel.LoadAsync(state);

            ApplyAnimations();
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            await ViewModel.UnloadAsync();
        }

        private void ApplyAnimations()
        {
            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            var scrollerPropertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);

            brush.BlurAmountExpression = compositor.AnimationExpression("Clamp(scroller.Translation.Y * parallaxFactor, 0, 360/10)")
                .Parameter("scroller", scrollerPropertySet)
                .Parameter("parallaxFactor", -0.05f)
                .Expression;

            compositor.AnimationExpression("Clamp(scroller.Translation.Y * parallaxFactor, -360/2, 0)")
                .Parameter("scroller", scrollerPropertySet)
                .Parameter("parallaxFactor", 0.25f)
                .Start(header, "Offset.Y");

            compositor.AnimationExpression("Lerp(0, -300, scroller.Translation.Y * parallaxFactor / -340.0)")
                .Parameter("scroller", scrollerPropertySet)
                .Parameter("parallaxFactor", 0.5f)
                .Start(pictureContainer, "Offset.Y");
        }
    }
}
