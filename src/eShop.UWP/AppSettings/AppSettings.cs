using System;

using Windows.Storage;
using Windows.ApplicationModel;

namespace eShop.UWP
{
    public class AppSettings
    {
        static private AppSettings _current = null;
        static public AppSettings Current => _current ?? (_current = new AppSettings());

        public ApplicationDataContainer LocalSettings => ApplicationData.Current.LocalSettings;

        public string DisplayName => Package.Current.DisplayName;

        public string Version
        {
            get
            {
                var ver = Package.Current.Id.Version;
                return $"{ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}";
            }
        }

        public string UserName
        {
            get => GetSettingsValue("UserName", default(String));
            set => SetSettingsValue("UserName", value);
        }

        public string WindowsHelloPublicKeyHint
        {
            get => GetSettingsValue("WindowsHelloPublicKeyHint", default(String));
            set => SetSettingsValue("WindowsHelloPublicKeyHint", value);
        }

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

        public string SqlConnectionString
        {
            get => GetSettingsValue("SqlConnectionString", @"Data Source=.\SQLExpress;Initial Catalog=eShopDb;Integrated Security=SSPI");
            set => SetSettingsValue("SqlConnectionString", value);
        }

        public bool IsNotificationQueueEnabled
        {
            get => GetSettingsValue<bool>("IsNotificationQueueEnabled", false);
            set => SetSettingsValue("IsNotificationQueueEnabled", value);
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
