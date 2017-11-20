using System;
using System.Linq;
using System.Threading.Tasks;

using Windows.ApplicationModel.Activation;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

using Microsoft.Toolkit.Uwp.Notifications;

using eShop.UWP.Activation;

namespace eShop.UWP.Services
{
    internal class LiveTileService : ActivationHandler<LaunchActivatedEventArgs>
    {
        private const string ImageBaseUri = "ms-appx:///Assets/Catalog/";

        public void EnableQueue()
        {
            if (!AppSettings.Current.IsNotificationQueueEnabled)
            {
                TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
                AppSettings.Current.IsNotificationQueueEnabled = true;
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

        protected override Task HandleInternalAsync(LaunchActivatedEventArgs args)
        {
            // If app is launched from a SecondaryTile, tile arguments property is contained in args.Arguments
            // var secondaryTileArguments = args.Arguments;

            // If app is launched from a LiveTile notification update, TileContent arguments property is contained in args.TileActivatedInfo.RecentlyShownNotifications
            // var tileUpdatesArguments = args.TileActivatedInfo.RecentlyShownNotifications;
            return Task.CompletedTask;
        }

        protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
        {
            return LaunchFromSecondaryTile(args) || LaunchFromLiveTileUpdate(args);
        }

        private bool LaunchFromSecondaryTile(LaunchActivatedEventArgs args)
        {
            // If app is launched from a SecondaryTile, tile arguments property is contained in args.Arguments
            // TODO WTS: Implement your own logic to determine if you can handle the SecondaryTile activation
            return false;
        }

        private bool LaunchFromLiveTileUpdate(LaunchActivatedEventArgs args)
        {
            // If app is launched from a LiveTile notification update, TileContent arguments property is contained in args.TileActivatedInfo.RecentlyShownNotifications
            // TODO WTS: Implement your own logic to determine if you can handle the LiveTile notification update activation
            return false;
        }

        private TileBindingContentPhotos GetTileBindingContentPhotos()
        {
            return new TileBindingContentPhotos
            {
                Images =
                {
                    new TileBasicImage { Source = $"{ImageBaseUri}101004.jpg" },
                    new TileBasicImage { Source = $"{ImageBaseUri}201006.jpg" },
                    new TileBasicImage { Source = $"{ImageBaseUri}401001.jpg" },
                    new TileBasicImage { Source = $"{ImageBaseUri}101002.jpg" },
                    new TileBasicImage { Source = $"{ImageBaseUri}201004.jpg" },
                }
            };
        }
    }
}
