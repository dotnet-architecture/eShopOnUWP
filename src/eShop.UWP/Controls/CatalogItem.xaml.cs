using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShop.UWP.Helpers;
using eShop.UWP.ViewModels.Catalog;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input.Inking;
using Windows.UI.Input.Inking.Analysis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace eShop.UWP.Controls
{
    public sealed partial class CatalogItem : UserControl
    {
        private const string StateString = "v";
        private const string SelectString = "o";
        private const string DeleteString = "x";
        private const string SemiDelete1String = "/";
        private const string SemiDelete2String = "\\";

        private InkAnalyzer analyzer = new InkAnalyzer();
        private IReadOnlyList<InkStroke> strokes;
        private InkAnalysisResult analysisResult;

        public static readonly DependencyProperty ItemViewModelProperty =
            DependencyProperty.Register("ItemViewModel",
                typeof(ItemViewModel),
                typeof(CatalogItem),
                new PropertyMetadata(null));

        public CatalogItem()
        {
            InitializeComponent();
        }

        public event EventHandler EditClick;

        public ItemViewModel ItemViewModel
        {
            get => (ItemViewModel)GetValue(ItemViewModelProperty);
            set => SetValue(ItemViewModelProperty, value);
        }

        private async void OnInkPresenterStrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
        {
            await RecognizeText(sender);
        }

        private async Task RecognizeText(InkPresenter inkPresenter)
        {
            strokes = inkPresenter.StrokeContainer.GetStrokes();

            if (strokes.Count > 0)
            {
                analyzer.AddDataForStrokes(strokes);

                foreach (var stroke in strokes)
                {
                    analyzer.SetStrokeDataKind(stroke.Id, InkAnalysisStrokeKind.Writing);
                }

                analysisResult = await analyzer.AnalyzeAsync();

                if (analysisResult.Status == InkAnalysisStatus.Updated)
                {
                    var words = analyzer.AnalysisRoot.FindNodes(InkAnalysisNodeKind.InkWord);
                    foreach (var word in words)
                    {
                        var concreteWord = (InkAnalysisInkWord)word;
                        foreach (string textValue in concreteWord.TextAlternates)
                        {
                            if (textValue.Equals(StateString, StringComparison.CurrentCultureIgnoreCase))
                            {
                                ItemViewModel?.SwitchState();
                            }
                            else if (textValue.Equals(SelectString, StringComparison.CurrentCultureIgnoreCase))
                            {
                                var gridView = this.FindParent<AdaptiveGridView>();
                                if (gridView?.SelectedItems?.Contains(ItemViewModel) ?? false)
                                {
                                    gridView?.SelectedItems?.Remove(ItemViewModel);
                                }
                                else
                                {
                                    gridView?.SelectedItems?.Add(ItemViewModel);
                                }
                            }
                            else if (textValue.Equals(DeleteString, StringComparison.CurrentCultureIgnoreCase))
                            {
                                inkPresenter.StrokesCollected -= OnInkPresenterStrokesCollected;
                                ItemViewModel?.Delete();
                            }
                            else if (textValue.Equals(SemiDelete1String, StringComparison.CurrentCultureIgnoreCase) ||
                                textValue.Equals(SemiDelete2String, StringComparison.CurrentCultureIgnoreCase))
                            {
                                return;
                            }
                            break;
                        }
                    }
                }
            }

            analyzer.ClearDataForAllStrokes();
            inkPresenter.StrokeContainer.Clear();
        }

        private void OnEditClick(object sender, RoutedEventArgs e)
        {
            EditClick?.Invoke(ItemViewModel, null);
        }

        private void OnRemoveClick(object sender, RoutedEventArgs e)
        {
            ItemViewModel?.Delete();
        }

        private void OnPointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Actions.Visibility = Visibility.Visible;
        }

        private void OnPointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Actions.Visibility = Visibility.Collapsed;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var inkCanvas = new InkCanvas();
            InkCanvasContent.Children.Clear();
            InkCanvasContent.Children.Add(inkCanvas);

            inkCanvas.InkPresenter.InputDeviceTypes =
                Windows.UI.Core.CoreInputDeviceTypes.Touch |
                Windows.UI.Core.CoreInputDeviceTypes.Pen;

            var color = Application.Current.Resources["InkColor"];

            var drawingAttributes = inkCanvas.InkPresenter.CopyDefaultDrawingAttributes();
            drawingAttributes.Color = (Color)color;
            drawingAttributes.IgnorePressure = false;
            drawingAttributes.Size = new Size(8, 8);

            inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
            inkCanvas.InkPresenter.StrokesCollected += OnInkPresenterStrokesCollected;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (InkCanvasContent.Children.FirstOrDefault() is InkCanvas inkCanvas)
            {
                inkCanvas.InkPresenter.StrokesCollected -= OnInkPresenterStrokesCollected;
                inkCanvas = null;
                InkCanvasContent.Children.Clear();
            }
        }
    }
}
