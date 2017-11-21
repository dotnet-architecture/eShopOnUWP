using System;

namespace eShop.WPF
{
    public class AppSettings
    {
        static private AppSettings _current = null;
        static public AppSettings Current => _current ?? (_current = new AppSettings());

        public string SqlConnectionString => @"Data Source=.\SQLExpress;Initial Catalog=eShopDb;Integrated Security=SSPI";
    }
}
