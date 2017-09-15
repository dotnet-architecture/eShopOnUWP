using System;
using System.Numerics;
using eShop.UWP.ViewModels.Base;
using eShop.UWP.ViewModels.Catalog;
using eShop.UWP.Views.Base;
using Microsoft.Toolkit.Uwp.UI.Animations.Expressions;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace eShop.UWP.Views.Catalog
{
    public sealed partial class ItemDetailView : PageBase
    {
        public ItemDetailViewModel ViewModel => DataContext as ItemDetailViewModel;

        private double _heightImageHead;
        private double _widthImageHead;
        private double _BorderContentImageTextHeight;
        private CompositionPropertySet _props;
        private CompositionPropertySet _scrollerPropertySet;
        private Compositor _compositor;
        private bool _animated = false;

        public ItemDetailView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            (DataContext as CustomViewModelBase).OnActivate(e.Parameter, e.NavigationMode == NavigationMode.Back);
        }

        private void OnCatalogItemEditClick(object sender, System.EventArgs e)
        {
            ViewModel?.ShowDetail(sender as ItemViewModel, AdaptiveGrid);
            MoveScrollToHead();
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            var animationService = ConnectedAnimationService.GetForCurrentView();
            var animation = animationService.GetAnimation(Constants.ConnectedAnimationKey);

            if (animation != null)
            {
                animation.TryStart(CoverImage, new UIElement[] { SetImage });
            }

            // Get the PropertySet that contains the scroll values from MyScrollViewer
            _scrollerPropertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(MyScrollviewer);
            _compositor = _scrollerPropertySet.Compositor;

            // Create a PropertySet that has values to be referenced in the ExpressionAnimations below
            _props = _compositor.CreatePropertySet();
            _props.InsertScalar("progress", 0);
            _props.InsertScalar("clampSize", 100);

            // Get references to our property sets for use with ExpressionNodes
            var scrollingProperties = _scrollerPropertySet.GetSpecializedReference<ManipulationPropertySetReferenceNode>();
            var props = _props.GetReference();
            var progressNode = props.GetScalarProperty("progress");
            var clampSizeNode = props.GetScalarProperty("clampSize");

            // Create and start an ExpressionAnimation to track scroll progress over the desired distance
            var progressAnimation = ExpressionFunctions.Clamp(-scrollingProperties.Translation.Y / clampSizeNode, 0, 1);
            _props.StartAnimation("progress", progressAnimation);

            // Get the backing visual for the header so that its properties can be animated
            var headerVisual = ElementCompositionPreview.GetElementVisual(Header);

            // Create and start an ExpressionAnimation to clamp the header's offset to keep it onscreen
            var headerTranslationAnimation = ExpressionFunctions.Conditional(progressNode < 1, 0, -scrollingProperties.Translation.Y - clampSizeNode);
            headerVisual.StartAnimation("Offset.Y", headerTranslationAnimation);

            // Create and start an ExpressionAnimation to scale the header during overpan
            var headerScaleAnimation = ExpressionFunctions.Lerp(1, 1.25f, ExpressionFunctions.Clamp(scrollingProperties.Translation.Y / 50, 0, 1));
            headerVisual.StartAnimation("Scale.X", headerScaleAnimation);
            headerVisual.StartAnimation("Scale.Y", headerScaleAnimation);

            //Set the header's CenterPoint to ensure the overpan scale looks as desired
            headerVisual.CenterPoint = new Vector3((float)(Header.ActualWidth / 2), (float)Header.ActualHeight, 0);
        }

        private void OnSaveClicked(object sender, RoutedEventArgs e)
        {
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate(Constants.ConnectedAnimationKey, CoverImage);
        }

        private void OnSelectItemClick(object sender, RoutedEventArgs e)
        {
            AdaptiveGrid.SelectedItems.Add((sender as FrameworkElement)?.DataContext);
        }

        private void OnRelatedItemEditClick(object sender, ItemClickEventArgs e)
        {
            MoveScrollToHead();
        }

        private void MoveScrollToHead()
        {
            var transform = this.TransformToVisual((UIElement)MyScrollviewer.Content);
            var position = transform.TransformPoint(new Point(0, 0));

            MyScrollviewer.ChangeView(null, -position.Y, null, false);
        }

        private void OnActivateSizeHeaderAnimation(object sender, ScrollViewerViewChangedEventArgs e)
        {
            
            if (_heightImageHead == 0)
            {
                _BorderContentImageTextHeight = BorderContentImageText.ActualHeight;
                _heightImageHead = ProfileImage.ActualHeight;
                _widthImageHead = ProfileImage.ActualWidth;
            }
            if (_animated && MyScrollviewer.VerticalOffset == 0)
            {
                _animated = false;
                var heightAnimation = SetSizeAnimation(400, (int)_heightImageHead, 200);
                var widthAnimation = SetSizeAnimation(180, (int)_widthImageHead, 200);
                CreateSizeAnimation(widthAnimation, heightAnimation);

                DetailText.Visibility = Visibility.Visible;
                PriceLastUpdateText.Visibility = Visibility.Visible;

                var GridheightAnimation = SetSizeAnimation(400, (int)_heightImageHead, 100);
                var GridWidthAnimation = SetSizeAnimation(180, (int)_widthImageHead, 100);
                CreateImageSizeAnimation(GridWidthAnimation, GridheightAnimation);

                BorderContentImageText.VerticalAlignment = VerticalAlignment.Top;

                EditData.Height = 500;
                TextContainer.Margin = new Thickness(24, 160, 0, 0);

                RectangleVisibility.Height = 100;

                ContainerImage.Width = 400;
                ContainerImage.Height = 320;
                MoveScrollToHead();
            }
            else if (MyScrollviewer.VerticalOffset > 0 && !_animated)
            {
                _animated = true;
                var heightAnimation = SetSizeAnimation((int)_heightImageHead, 400, 200);
                var widthAnimation = SetSizeAnimation((int)_widthImageHead, 220, 200);
                CreateSizeAnimation(widthAnimation, heightAnimation);

                DetailText.Visibility = Visibility.Collapsed;
                PriceLastUpdateText.Visibility = Visibility.Collapsed;

                var GridheightAnimation = SetSizeAnimation((int)_heightImageHead, 400, 200);
                var GridWidthAnimation = SetSizeAnimation((int)_widthImageHead, 220, 200);
                CreateImageSizeAnimation(GridWidthAnimation, GridheightAnimation);

                BorderContentImageText.VerticalAlignment = VerticalAlignment.Top;
                EditData.Height = 300;
                TextContainer.Margin = new Thickness(24, 160, 0, -40);

                RectangleVisibility.Height = 300;

                ContainerImage.Width = 250;
                ContainerImage.Height = 150;
                ContainerImage.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }

        private void CreateImageSizeAnimation(DoubleAnimation widthAnimation, DoubleAnimation heightAnimation)
        {
            Storyboard.SetTargetProperty(widthAnimation, "Width");
            Storyboard.SetTargetProperty(heightAnimation, "Height");

            Storyboard.SetTarget(widthAnimation, ProfileImage);
            Storyboard.SetTarget(heightAnimation, ProfileImage);

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(widthAnimation);
            storyboard.Children.Add(heightAnimation);
            storyboard.Begin();
        }

        private void CreateImageSizeAnimation(DoubleAnimation heightAnimation)
        {
            Storyboard.SetTargetProperty(heightAnimation, "Height");

            Storyboard.SetTarget(heightAnimation, CoverImage);

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(heightAnimation);
            storyboard.Begin();
        }

        private void CreateSizeAnimation(DoubleAnimation widthAnimation, DoubleAnimation heightAnimation)
        {
            Storyboard.SetTargetProperty(widthAnimation, "Width");
            Storyboard.SetTargetProperty(heightAnimation, "Height");

            Storyboard.SetTarget(widthAnimation, ColumnImage);
            Storyboard.SetTarget(heightAnimation, ColumnImage);

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(widthAnimation);
            storyboard.Children.Add(heightAnimation);
            storyboard.Begin();
        }

        private DoubleAnimation SetSizeAnimation(int fromSize, int toSize, int milliseconds)
        {
            return new DoubleAnimation
            {
                From = fromSize,
                To = toSize,
                Duration = TimeSpan.FromMilliseconds(milliseconds),
                EnableDependentAnimation = true
            };
        }
    }
}
