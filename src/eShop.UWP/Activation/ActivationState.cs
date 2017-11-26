using System;

using eShop.UWP.ViewModels;

namespace eShop.UWP.Activation
{
    public class ActivationState
    {
        static public readonly ActivationState Default = new ActivationState(typeof(CatalogViewModel), new CatalogState());

        public ActivationState()
        {
        }
        public ActivationState(Type viewModel, object parameter = null)
        {
            ViewModel = viewModel;
            Parameter = parameter;
        }

        public Type ViewModel { get; set; }
        public object Parameter { get; set; }
    }
}
