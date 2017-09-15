using Windows.UI.Notifications;
using eShop.Domain.Models;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.ApplicationModel.Activation;
using eShop.UWP.Activation;
using System.Threading.Tasks;
using eShop.UWP.ViewModels.Catalog;
using System;

namespace eShop.UWP.Services
{
    internal class ToastNotificationsService : ActivationHandler<ToastNotificationActivatedEventArgs>
    {
        public void ShowToastNotification(string title, CatalogItem item)
        {
            var content = GenerateToastContent(title, item);
            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(content.GetXml()));
        }

        protected override async Task HandleInternalAsync(ToastNotificationActivatedEventArgs args, Type defaultNavItem)
        {
            if (IsAuthenticated)
            {
                NavigationService.Navigate(typeof(CatalogViewModel).FullName, null);
            }
            else
            {
                NavigationService.Navigate(defaultNavItem.FullName, null);
            }

            await Task.CompletedTask;
        }

        private static ToastContent GenerateToastContent(string title, CatalogItem item)
        {
            var binding = new ToastBindingGeneric
            {
                Children =
                {
                    new AdaptiveText
                    {
                        Text = title
                    },
                    new AdaptiveText
                    {
                        Text = item.Name
                    },
                    new AdaptiveText
                    {
                        Text = item.Description
                    }
                }
            };

            if (!string.IsNullOrEmpty(item.PictureUri))
            {
                binding.Children.Add(new AdaptiveImage
                {
                    Source = item.PictureUri
                });
            }

            return new ToastContent
            {
                Visual = new ToastVisual
                {
                    BindingGeneric = binding
                }
            };
        }
    }
}
