using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.Media.SpeechRecognition;

namespace eShop.Cortana
{
    public sealed class VoiceCommandService
    {
        public string FilterVoiceCommand { get; set; }
        private CatalogVoiceCommand _catalogVoiceCommand;

        public CatalogVoiceCommand RunCommand(VoiceCommandActivatedEventArgs cmd)
        {
            var speechRecognitionResult = cmd.Result;
            var commandName = speechRecognitionResult.RulePath[0];
            var commandMode = SemanticInterpretation("commandMode", speechRecognitionResult);
            var textSpoken = speechRecognitionResult.Text;

            switch (commandName)
            {
                case "showItemsSearch":
                    FilterVoiceCommand = SemanticInterpretation("catalogType", speechRecognitionResult);
                    _catalogVoiceCommand = new CatalogVoiceCommand
                    {
                        VoiceCommand = speechRecognitionResult.ToString(),
                        CommandMode = commandMode.ToString(),
                        TextSpoken = textSpoken.ToString(),
                        Value = FilterVoiceCommand.ToString()
                    };
                    break;
            }
            return _catalogVoiceCommand;
        }

        private string SemanticInterpretation(string interpretationKey, SpeechRecognitionResult speechRecognitionResult)
        {
            return speechRecognitionResult.SemanticInterpretation.Properties[interpretationKey].FirstOrDefault();
        }

        public CatalogVoiceCommand SelectDetail(ProtocolActivatedEventArgs activationArgs)
        {
            var commandArgs = activationArgs;
            var decoder = new Windows.Foundation.WwwFormUrlDecoder(commandArgs.Uri.Query);
            FilterVoiceCommand = decoder.GetFirstValueByName("LaunchContext");

            _catalogVoiceCommand = new CatalogVoiceCommand
            {
                VoiceCommand = "protocolLaunch",
                CommandMode = "text",
                TextSpoken = "filter",
                Value = FilterVoiceCommand
            };

            return _catalogVoiceCommand;
        }
    }
}
