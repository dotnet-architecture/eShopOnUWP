using System;

using Windows.Storage;

namespace eShop.UWP
{
    public class AppSettings
    {
        static private AppSettings _current = null;
        static public AppSettings Current => _current ?? (_current = new AppSettings());

        public ApplicationDataContainer LocalSettings => ApplicationData.Current.LocalSettings;

        public DataProviderType DataProvider
        {
            get => (DataProviderType)GetSettingsValue("DataProvider", (int)DataProviderType.Local);
            set => LocalSettings.Values["DataProvider"] = (int)value;
        }

        public string ServiceUrl
        {
            get => GetSettingsValue("ServiceUrl", "http://localhost:5001");
            set => SetSettingsValue("ServiceUrl", value);
        }

        private TResult GetSettingsValue<TResult>(string name, TResult defaultValue)
        {
            try
            {
                if (!LocalSettings.Values.ContainsKey(name))
                {
                    LocalSettings.Values[name] = defaultValue;
                }
                return (TResult)LocalSettings.Values[name];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return defaultValue;
            }
        }
        private void SetSettingsValue(string name, object value)
        {
            LocalSettings.Values[name] = value;
        }
    }
}
