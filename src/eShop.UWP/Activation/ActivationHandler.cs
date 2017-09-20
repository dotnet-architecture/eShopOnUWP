using System;
using System.Threading.Tasks;

namespace eShop.UWP.Activation
{
    internal abstract class ActivationHandler
    {
        public abstract bool CanHandle(object args);
        public abstract Task HandleAsync(object args, Type defaultNavItem);
    }
}

