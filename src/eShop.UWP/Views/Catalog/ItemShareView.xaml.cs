using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Input.Inking;
using Windows.UI.Core;

using eShop.UWP.ViewModels;
using eShop.UWP.Models;

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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var state = (e.Parameter as ItemShareState) ?? new ItemShareState(new CatalogItemModel());
            ViewModel.Load(state);

            Bindings.Update();
        }
    }
}
