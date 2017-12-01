using System;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Hosting;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

using eShop.UWP.ViewModels;
using eShop.UWP.Animations;
using eShop.Providers;

namespace eShop.UWP.Views
{
    public sealed partial class ItemDetailView : Page
    {
        public ItemDetailView()
        {
            InitializeComponent();
            DataContext = new ItemDetailViewModel();
            Loaded += OnLoaded;
        }

        public ItemDetailViewModel ViewModel => DataContext as ItemDetailViewModel;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ApplyAnimations();

            var state = (e.Parameter as ItemDetailState) ?? new ItemDetailState();
            await ViewModel.LoadAsync(state);
        }

        private void OnLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("ItemSelected");
            if (imageAnimation != null)
            {
                imageAnimation.TryStart(pictureContainer, new UIElement[] { itemContainer });
            }
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ItemSelectedBack", picture);
            }

            base.OnNavigatingFrom(e);
            await ViewModel.UnloadAsync();
        }

        private void ApplyAnimations()
        {
            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            var scrollerPropertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);

            var anim = compositor.CreateScalarKeyFrameAnimation();
            anim.InsertKeyFrame(0.0f, -340.0f);
            anim.InsertKeyFrame(1.0f, 0.0f);
            anim.Duration = TimeSpan.FromSeconds(0.250);
            ElementCompositionPreview.GetElementVisual(headerOffset).StartAnimation("Offset.Y", anim);

            brush.BlurAmountExpression = compositor.CreateExpressionWrapper("Clamp(scroller.Translation.Y * parallaxFactor, 0, 360/10)")
                .Parameter("scroller", scrollerPropertySet)
                .Parameter("parallaxFactor", -0.05f)
                .Expression;

            compositor.CreateExpressionWrapper("Clamp(scroller.Translation.Y * parallaxFactor, -360/2, 0)")
                .Parameter("scroller", scrollerPropertySet)
                .Parameter("parallaxFactor", 0.25f)
                .Start(header, "Offset.Y");

            compositor.CreateExpressionWrapper("Lerp(0, -300, scroller.Translation.Y * parallaxFactor / -340.0)")
                .Parameter("scroller", scrollerPropertySet)
                .Parameter("parallaxFactor", 0.5f)
                .Start(pictureContainer, "Offset.Y");
        }
    }
}
