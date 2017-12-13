using System;
using System.Threading.Tasks;

using Windows.ApplicationModel.Activation;

using GalaSoft.MvvmLight.Ioc;

using eShop.Cortana;
using eShop.UWP.Activation;

namespace eShop.UWP.Services
{
    internal class VoiceCommandActivationService : ActivationHandler<VoiceCommandActivatedEventArgs>
    {
        protected override Task<ActivationState> HandleInternalAsync(VoiceCommandActivatedEventArgs args)
        {
            var voiceCommandService = SimpleIoc.Default.GetInstance<VoiceCommandService>();
            var filterByVoiceCommand = voiceCommandService.RunCommand(args);

            return Task.FromResult<ActivationState>(null);
        }
    }
}
