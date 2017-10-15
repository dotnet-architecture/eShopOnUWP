using eShop.Cortana;
using eShop.Providers;
using eShop.Providers.Contracts;
using eShop.UWP.Services;
using eShop.UWP.ViewModels.Catalog;
using eShop.UWP.ViewModels.Login;
using eShop.UWP.ViewModels.Shell;
using eShop.UWP.Views;
using eShop.UWP.Views.Catalog;
using eShop.UWP.Views.Login;
using eShop.UWP.Views.Shell;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace eShop.UWP.ViewModels.Base
{
    public class ViewModelLocator
    {
        NavigationServiceEx _navigationService = new NavigationServiceEx();

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register(() => _navigationService);
            SimpleIoc.Default.Register<ShellViewModel>();
            SimpleIoc.Default.Register<SignInWithHelloViewModel>();
            SimpleIoc.Default.Register<SignInWithPasswordViewModel>();
            SimpleIoc.Default.Register<ItemsGridViewModel>();
            SimpleIoc.Default.Register<ItemsListViewModel>();
            SimpleIoc.Default.Register<VoiceCommandService>();
            SimpleIoc.Default.Register<ICatalogProvider, LocalCatalogProvider>();
            SimpleIoc.Default.Register<IOrdersProvider, OrdersProvider>();

            Register<LoginViewModel, LoginView>();
            Register<CatalogViewModel, CatalogView>();
            Register<StatisticsViewModel, StatisticsView>();
            Register<ItemsListViewModel, ItemsListView>();
            Register<ShellViewModel, ShellView>();
            Register<ItemDetailViewModel, ItemDetailView>();
        }

        public ShellViewModel ShellViewModel => ServiceLocator.Current.GetInstance<ShellViewModel>();

        public LoginViewModel LoginViewModel => ServiceLocator.Current.GetInstance<LoginViewModel>();

        public ItemsListViewModel ItemsListViewViewModel => ServiceLocator.Current.GetInstance<ItemsListViewModel>();

        public StatisticsViewModel StatisticsViewViewModel => ServiceLocator.Current.GetInstance<StatisticsViewModel>();

        public CatalogViewModel CatalogViewModel => ServiceLocator.Current.GetInstance<CatalogViewModel>();

        public StatisticsViewModel StatisticsViewModel => ServiceLocator.Current.GetInstance<StatisticsViewModel>();

        public ItemDetailViewModel ItemDetailViewModel => ServiceLocator.Current.GetInstance<ItemDetailViewModel>();

        public void Register<VM, V>() where VM : class
        {
            SimpleIoc.Default.Register<VM>();

            _navigationService.Configure(typeof(VM).FullName, typeof(V));
        }
    }
}

