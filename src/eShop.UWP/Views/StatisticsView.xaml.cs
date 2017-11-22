using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using eShop.UWP.ViewModels;

namespace eShop.UWP.Views
{
    public sealed partial class StatisticsView : Page
    {
        public StatisticsView()
        {
            this.InitializeComponent();
        }

        private StatisticsViewModel ViewModel => DataContext as StatisticsViewModel;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.Load();
        }
    }
}
