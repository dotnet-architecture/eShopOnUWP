namespace eShop.SqlProvider.Properties
{
    public class Settings
    {
        static Settings()
        {
            Default = new Settings();
        }

        static public Settings Default { get; private set; }

        public string ConnectionString { get; set; }
    }
}
