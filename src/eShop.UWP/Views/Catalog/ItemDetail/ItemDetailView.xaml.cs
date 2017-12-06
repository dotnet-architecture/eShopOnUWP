using System;
using System.Linq;
using System.Collections.Generic;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Composition;

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

            PrepareAnimations();

            var state = (e.Parameter as ItemDetailState) ?? new ItemDetailState();
            await ViewModel.LoadAsync(state);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("ItemSelected");
            if (imageAnimation != null)
            {
                imageAnimation.TryStart(picture, new UIElement[] { group1, group2 });
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

        private void PrepareAnimations()
        {
            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            var scrollerPropertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);

            brush.BlurAmountExpression = compositor.CreateExpressionWrapper("Clamp(scroller.Translation.Y * parallaxFactor, 0, 200/10)")
                .Parameter("scroller", scrollerPropertySet)
                .Parameter("parallaxFactor", -0.05f)
                .Expression;

            headerImage.TranslateY(250, -338, 0);

            scrollViewer.StopElementAtOffset(header, 200);
            scrollViewer.StopElementAtOffset(picture, 338);
            scrollViewer.StopElementAtOffset(group1, 200);
            scrollViewer.StopElementAtOffset(aside, 338);
        }
    }
}
