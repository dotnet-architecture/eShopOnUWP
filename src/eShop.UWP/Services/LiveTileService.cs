using System;
using System.Linq;
using System.Threading.Tasks;
using eShop.UWP.Activation;
using eShop.UWP.Helpers;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace eShop.UWP.Services
{
    internal class LiveTileService : ActivationHandler<LaunchActivatedEventArgs>
    {
        private const string QueueEnabledKey = "NotificationQueueEnabled";
        private const string ImageBaseUri = "ms-appx:///Assets/Tiles/";

        public async Task EnableQueueAsync()
        {
            var queueEnabled = await ApplicationData.Current.LocalSettings.ReadAsync<bool>(QueueEnabledKey);
            if (!queueEnabled)
            {
                TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
                await ApplicationData.Current.LocalSettings.SaveAsync(QueueEnabledKey, true);
            }
        }

        public void UpdateTile(TileNotification notification)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

        public async Task<bool> PinSecondaryTileAsync(SecondaryTile tile, bool allowDuplicity = false)
        {
            if (!await IsAlreadyPinnedAsync(tile) || allowDuplicity)
            {
                return await tile.RequestCreateAsync();
            }
            return false;
        }

        public void TileUpdate()
        {
            var content = new TileContent
            {
                Visual = new TileVisual
                {

                    TileMedium = new TileBinding
                    {
                        Branding = TileBranding.NameAndLogo,
                        Content = GetTileBindingContentPhotos()
                    },
                    TileWide = new TileBinding
                    {
                        Branding = TileBranding.NameAndLogo,
                        Content = GetTileBindingContentPhotos()
                    }
                }
            };

            var notification = new TileNotification(content.GetXml());
            UpdateTile(notification);
        }

        private async Task<bool> IsAlreadyPinnedAsync(SecondaryTile tile)
        {
            var secondaryTiles = await SecondaryTile.FindAllAsync();
            return secondaryTiles.Any(t => t.Arguments == tile.Arguments);
        }

        protected override async Task HandleInternalAsync(LaunchActivatedEventArgs args, Type defaultNavItem)
        {
            await Task.CompletedTask;
        }

        protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
        {
            return LaunchFromSecondaryTile(args) || LaunchFromLiveTileUpdate(args);
        }

        private bool LaunchFromSecondaryTile(LaunchActivatedEventArgs args)
        {
            return false;
        }

        private bool LaunchFromLiveTileUpdate(LaunchActivatedEventArgs args)
        {
            return false;
        }

        private TileBindingContentPhotos GetTileBindingContentPhotos()
        {
            return new TileBindingContentPhotos
            {
                Images =
                {
                    new TileBasicImage { Source = $"{ImageBaseUri}1.png" },
                    new TileBasicImage { Source = $"{ImageBaseUri}2.png" },
                    new TileBasicImage { Source = $"{ImageBaseUri}3.png" },
                    new TileBasicImage { Source = $"{ImageBaseUri}4.png" },
                    new TileBasicImage { Source = $"{ImageBaseUri}5.png" }
                }
            };
        }
    }
}

