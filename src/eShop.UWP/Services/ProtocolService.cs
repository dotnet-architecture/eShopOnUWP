using eShop.Cortana;
using eShop.UWP.Activation;
using eShop.UWP.ViewModels.Catalog;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace eShop.UWP.Services
{
    internal class ProtocolService : ActivationHandler<ProtocolActivatedEventArgs>
    {
        protected override async Task HandleInternalAsync(ProtocolActivatedEventArgs args, Type defaultNavItem)
        {
            var voiceCommandService = SimpleIoc.Default.GetInstance<VoiceCommandService>();
            var itemSelected = voiceCommandService.SelectDetail(args);

            if (IsAuthenticated)
            {
                NavigationService.Navigate(typeof(ItemDetailViewModel).FullName, itemSelected);
            }
            else
            {
                NavigationService.Navigate(defaultNavItem.FullName, itemSelected);
            }

            await Task.CompletedTask;
        }
    }
}
