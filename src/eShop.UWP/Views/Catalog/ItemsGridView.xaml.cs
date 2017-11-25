using System;
using System.Linq;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Composition;

using eShop.UWP.ViewModels;

namespace eShop.UWP.Views
{
    public sealed partial class ItemsGridView : UserControl
    {
        private ExpressionAnimation _expression = null;

        public ItemsGridView()
        {
            this.InitializeComponent();
        }

        public ItemsGridViewModel ViewModel => DataContext as ItemsGridViewModel;

        private ExpressionAnimation Expression => _expression ?? (_expression = CreateExpression());

        public void Initialize()
        {
            ViewModel.ItemsControl = gridView;
            ViewModel.BarItemsControl = target;
        }

        public async Task DoConnectedAnimationAsync(int id)
        {
            var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("ItemSelectedBack");
            if (animation != null)
            {
                if (id > 0)
                {
                    var item = ViewModel.Items.Where(r => r.Id == id).FirstOrDefault();
                    if (item != null)
                    {
                        gridView.ScrollIntoView(item);
                        await Task.Delay(10);
                        await gridView.TryStartConnectedAnimationAsync(animation, item, "container");
                        return;
                    }
                }
                animation.Cancel();
            }
        }

        private ExpressionAnimation CreateExpression()
        {
            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            var scrollViewer = gridView.GetChildOfType<ScrollViewer>();
            var scrollerPropertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);

            var _expression = compositor.CreateExpressionAnimation("Vector3(0, (-scroller.Translation.Y - container.Offset.Y + 300) * 0.05, 0)");
            _expression.SetReferenceParameter("scroller", scrollerPropertySet);
            return _expression;
        }

        private void OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (!args.InRecycleQueue)
            {
                var container = args.ItemContainer;
                var containerVisual = ElementCompositionPreview.GetElementVisual(container);

                var item = container.GetChildOfType<Border>();
                var itemVisual = ElementCompositionPreview.GetElementVisual(item);

                Expression.SetReferenceParameter("container", containerVisual);
                itemVisual.StartAnimation("Offset", Expression);
            }
        }
    }
}
