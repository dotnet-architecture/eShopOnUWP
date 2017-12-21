using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Input.Inking;
using Windows.UI.Core;

using eShop.UWP.ViewModels;
using eShop.UWP.Models;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;

namespace eShop.UWP.Views
{
    public sealed partial class ItemShareView : Page
    {
        public ItemShareView()
        {
            InitializeComponent();
            ViewModel = new ItemShareViewModel();
            DataContext = ViewModel;
            InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Touch;
        }

        public ItemShareViewModel ViewModel { get; private set; }

        private InkPresenter InkPresenter => inkCanvas.InkPresenter;

        public UIHelper Helper => UIHelper.Current;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += OnDataRequested;

            var state = (e.Parameter as ItemShareState) ?? new ItemShareState(new CatalogItemModel());
            ViewModel.Load(state);

            Bindings.Update();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested -= OnDataRequested;
        }

        private async void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var deferral = args.Request.GetDeferral();

            var dataRequest = args.Request;
            dataRequest.Data.Properties.Title = ViewModel.Item.Name;
            dataRequest.Data.SetBitmap(RandomAccessStreamReference.CreateFromStream(await document.RenderAsync()));

            deferral.Complete();
        }

        private void OnShareClick(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }
    }
}
