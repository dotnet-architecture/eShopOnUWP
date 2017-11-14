using System;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
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
