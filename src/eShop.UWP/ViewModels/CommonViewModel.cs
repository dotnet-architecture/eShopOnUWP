using System;

using Windows.UI.Xaml;

using Microsoft.Practices.ServiceLocation;

using GalaSoft.MvvmLight;

using eShop.UWP.Services;

namespace eShop.UWP.ViewModels
{
    public class CommonViewModel : ViewModelBase
    {
        protected NavigationServiceEx NavigationService => ServiceLocator.Current.GetInstance<NavigationServiceEx>();

        virtual public bool AlwaysShowHeader => true;

        virtual public DataTemplate HeaderTemplate => App.Current.Resources["CommonHeaderTemplate"] as DataTemplate;

        private string _headerText;
        public string HeaderText
        {
            get { return _headerText; }
            set { Set(ref _headerText, value); }
        }
    }
}
