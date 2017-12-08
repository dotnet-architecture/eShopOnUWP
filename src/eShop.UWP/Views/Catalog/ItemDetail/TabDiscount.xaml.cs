using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using eShop.UWP.ViewModels;

namespace eShop.UWP.Views
{
    public sealed partial class TabDiscount : UserControl
    {
        public TabDiscount()
        {
            this.InitializeComponent();

            var date = dateFrom.MinDate;
            var date2 = dateFrom.MaxDate;

        }

        public ItemDetailViewModel ViewModel { get; set; }

        public UIHelper Helper => UIHelper.Current;
    }
}
