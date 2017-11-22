using System;
using System.Threading.Tasks;

using Windows.UI.Notifications;
using Windows.ApplicationModel.Activation;

using Microsoft.Toolkit.Uwp.Notifications;

using eShop.UWP.Models;
using eShop.UWP.ViewModels;
using eShop.UWP.Activation;
using eShop.UWP.Helpers;
using eShop.Providers;

namespace eShop.UWP.Services
{
    internal class ToastNotificationsService : ActivationHandler<ToastNotificationActivatedEventArgs>
    {
        static public ToastNotificationsService Current => Singleton<ToastNotificationsService>.Instance;

        public void ShowToastNotification(string title, CatalogItemModel item)
        {
            var content = GenerateToastContent(title, item);
            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(content.GetXml()));
        }

        protected override async Task HandleInternalAsync(ToastNotificationActivatedEventArgs args)
        {
            if (NavigationService.Frame != null)
            {
                if (Int32.TryParse(args.Argument, out int id))
                {
                    var provider = new CatalogProvider();
                    var item = await provider.GetItemByIdAsync(id);
                    if (item != null)
                    {
                        NavigationService.Navigate(typeof(ItemDetailViewModel).FullName, new ItemDetailState(item));
                    }
                }
            }
        }

        private static ToastContent GenerateToastContent(string title, CatalogItemModel item)
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
                Launch = item.Id.ToString(),
                Visual = new ToastVisual
                {
                    BindingGeneric = binding
                }
            };
        }
    }
}
