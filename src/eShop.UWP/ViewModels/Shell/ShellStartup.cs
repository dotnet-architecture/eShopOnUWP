using System;

using Microsoft.Practices.ServiceLocation;

using eShop.UWP.Services;
using eShop.UWP.ViewModels.Shell;
using eShop.Cortana;
using eShop.Providers;

namespace eShop.UWP.ViewModels
{
    static public class ShellStartup
    {
        static private NavigationServiceEx NavigationService => ServiceLocator.Current.GetInstance<NavigationServiceEx>();

        static public async void Start(CatalogVoiceCommand catalogVoiceCommand)
        {
            var result = await SwitchProvider.IsCurrentProviderAvailableAsync();
            if (result.IsOk)
            {
                NavigationService.Navigate(typeof(ShellViewModel).FullName, catalogVoiceCommand);
            }
            else
            {
                await DialogBox.ShowAsync("Error connecting with current provider", result.Message);
                NavigationService.Navigate(typeof(SettingsViewModel).FullName, catalogVoiceCommand);
            }
        }
    }
}
