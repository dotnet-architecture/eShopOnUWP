using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using eShop.UWP.ViewModels;

namespace eShop.UWP.Views
{
    public sealed partial class StatisticsPage : Page
    {
        public StatisticsPage()
        {
            this.InitializeComponent();
        }

        private StatisticsViewModel ViewModel => DataContext as StatisticsViewModel;
    }
}
