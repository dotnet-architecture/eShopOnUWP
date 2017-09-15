using eShop.UWP.Services;
using eShop.UWP.Views.Login;
using System;
using System.Threading.Tasks;

namespace eShop.UWP.Activation
{
    internal abstract class ActivationHandler<T> : ActivationHandler where T : class
    {
        protected NavigationServiceEx NavigationService => Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationServiceEx>();

        protected bool IsAuthenticated => NavigationService.Frame.CurrentSourcePageType != null && NavigationService.Frame.CurrentSourcePageType != typeof(LoginView);


        protected abstract Task HandleInternalAsync(T args, Type defaultNavItem);

        public override async Task HandleAsync(object args, Type defaultNavItem)
        {
            await HandleInternalAsync(args as T, defaultNavItem);
        }

        public override bool CanHandle(object args)
        {
            return args is T && CanHandleInternal(args as T);
        }

        protected virtual bool CanHandleInternal(T args)
        {
            return true;
        }
    }
}
