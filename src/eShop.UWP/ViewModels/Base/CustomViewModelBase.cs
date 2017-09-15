using eShop.UWP.Services;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;

namespace eShop.UWP.ViewModels.Base
{
    public class CustomViewModelBase : ViewModelBase
    {
        protected object Parameter;

        protected NavigationServiceEx NavigationService => ServiceLocator.Current.GetInstance<NavigationServiceEx>();

        public virtual void OnActivate(object parameter, bool isBack)
        {
            Parameter = parameter;
        }

        public virtual void OnDeactivate()
        {
        }

        public void CleanBackStack()
        {
            NavigationService.CleanBackStack();
        }
    }
}
