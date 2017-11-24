using System;
using System.Threading.Tasks;

using Windows.ApplicationModel.Activation;

using Microsoft.Practices.ServiceLocation;

using eShop.UWP.Services;

namespace eShop.UWP.Activation
{
    // For more information on application activation see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/activation.md
    internal abstract class ActivationHandler
    {
        protected NavigationServiceEx NavigationService => ServiceLocator.Current.GetInstance<NavigationServiceEx>();

        public abstract bool CanHandle(IActivatedEventArgs args);

        public abstract Task<ActivationState> HandleAsync(IActivatedEventArgs args);
    }

    internal abstract class ActivationHandler<T> : ActivationHandler where T : class, IActivatedEventArgs
    {
        public override bool CanHandle(IActivatedEventArgs args)
        {
            return args is T && CanHandleInternal(args as T);
        }

        protected virtual bool CanHandleInternal(T args)
        {
            return true;
        }

        public override async Task<ActivationState> HandleAsync(IActivatedEventArgs args)
        {
            return await HandleInternalAsync(args as T);
        }

        protected abstract Task<ActivationState> HandleInternalAsync(T args);
    }
}
