using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Globalization;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources.Core;
using Windows.ApplicationModel.VoiceCommands;

using eShop.Domain.Models;
using eShop.Providers;
using eShop.Providers.Contracts;

namespace eShop.Cortana
{
    public sealed class BackgroundVoiceCommandService : IBackgroundTask
    {
        private BackgroundTaskDeferral _serviceDeferral;
        private VoiceCommandServiceConnection _voiceServiceConnection;
        private ResourceMap _cortanaResourceMap;
        private ResourceContext _cortanaContext;
        private DateTimeFormatInfo _dateFormatInfo;
        private ICatalogProvider _catalogProvider;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _serviceDeferral = taskInstance.GetDeferral();

            taskInstance.Canceled += OnTaskCanceled;

            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            _cortanaResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
            _cortanaContext = ResourceContext.GetForViewIndependentUse();
            _dateFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;

            if (triggerDetails != null && triggerDetails.Name == "BackgroundVoiceCommandService")
            {
                try
                {
                    _voiceServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);
                    _voiceServiceConnection.VoiceCommandCompleted += VoiceCommandCompleted;

                    var voiceCommand = await _voiceServiceConnection.GetVoiceCommandAsync();

                    switch (voiceCommand.CommandName)
                    {
                        case "showItemsSearch":
                            {
                                var filter = voiceCommand.Properties["catalogType"][0];
                                await SendCompletionMessageForFilter(filter);
                                break;
                            }
                        default:
                            LaunchAppInForeground();
                            break;
                    }
                }
                finally
                {
                    _serviceDeferral?.Complete();
                }
            }
        }

        private void VoiceCommandCompleted(VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args)
        {
            _serviceDeferral?.Complete();
        }

        private async Task SendCompletionMessageForFilter(string filter)
        {
            var loadingSearchByType = string.Format(
                   _cortanaResourceMap.GetValue("Cortana_Loading", _cortanaContext).ValueAsString, filter);
            await ShowProgressScreen(loadingSearchByType);

            var store = new CatalogItem();

            // TODOX: Get depending on configuration configuration
            _catalogProvider = new LocalCatalogProvider();
            var items = await _catalogProvider?.GetItemsByVoiceCommandAsync(filter);

            var userMessage = new VoiceCommandUserMessage();

            var ListContentTiles = new List<VoiceCommandContentTile>();

            if (items == null || !items.Any())
            {
                var foundNoSearchByType = string.Format(_cortanaResourceMap.GetValue("Cortana_foundNoSearchByType", _cortanaContext).ValueAsString, filter);
                userMessage.DisplayMessage = foundNoSearchByType;
                userMessage.SpokenMessage = foundNoSearchByType;
            }
            else
            {
                int cont = 1;

                foreach (CatalogItem item in items.Take(10))
                {
                    var typeTile = new VoiceCommandContentTile();
                    typeTile.ContentTileType = VoiceCommandContentTileType.TitleWithText;

                    typeTile.AppLaunchArgument = item.Id.ToString();
                    typeTile.Title = item.Name;
                    typeTile.TextLine1 = $"{item.Price.ToString()}$";

                    ListContentTiles.Add(typeTile);
                    cont++;
                }
            }

            var message = WaitingForResult(filter, items.Count());
            userMessage.DisplayMessage = message;
            userMessage.SpokenMessage = message;
            var response = VoiceCommandResponse.CreateResponse(userMessage, ListContentTiles);
            await _voiceServiceConnection.ReportSuccessAsync(response);
        }

        private string WaitingForResult(string filter, int count)
        {
            return count > 0 ? string.Format(_cortanaResourceMap.GetValue("Cortana_findSomeElements", _cortanaContext).ValueAsString, filter) + count:
                string.Format(_cortanaResourceMap.GetValue("Cortana_foundNoSearchByType", _cortanaContext).ValueAsString, filter);
        }

        private async Task<string> DownloadFileAsync(Uri uri, string filename)
        {
            using (var fileStream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(filename, CreationCollisionOption.ReplaceExisting))
            {
                var webStream = await new HttpClient().GetStreamAsync(uri);
                await webStream.CopyToAsync(fileStream);
                webStream.Dispose();
            }
            return (await ApplicationData.Current.LocalFolder.GetFileAsync(filename)).Path;
        }

        private async Task ShowProgressScreen(string message)
        {
            var userProgressMessage = new VoiceCommandUserMessage();
            userProgressMessage.DisplayMessage = userProgressMessage.SpokenMessage = message;

            var response = VoiceCommandResponse.CreateResponse(userProgressMessage);
            await _voiceServiceConnection.ReportProgressAsync(response);
        }

        private async void LaunchAppInForeground(string appLaunchArgument = "")
        {
            var userMessage = new VoiceCommandUserMessage { SpokenMessage = string.Empty };
            var response = VoiceCommandResponse.CreateResponse(userMessage);
            response.AppLaunchArgument = appLaunchArgument ?? string.Empty;

            await _voiceServiceConnection.RequestAppLaunchAsync(response);
        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            System.Diagnostics.Debug.WriteLine("Task cancelled, clean up");
            _serviceDeferral?.Complete();
        }
    }
}
