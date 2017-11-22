using System;
using System.IO;
using System.Collections.Generic;

using eShop.Data;
using eShop.UWP.Helpers;
using eShop.UWP.Services;

namespace eShop.Providers
{
    public class LocalCatalogDb : JsonDb
    {
        const string CURRENT_VERSION = "1.0";
        const string DEFAULT_FILENAME = "LocalCatalogDb.json";

        public LocalCatalogDb(string fileName = DEFAULT_FILENAME) : base(fileName)
        {
            if (!File.Exists(base.FilePath) || Version != CURRENT_VERSION)
            {
                string json = Resources.LoadString("CatalogDb.CatalogDb.json");
                Deserialize(json);
                Version = CURRENT_VERSION;
                SaveChanges();
            }
        }

        static public void ResetData(string fileName = DEFAULT_FILENAME)
        {
            File.Delete(GetFilePath(fileName));
        }

        public string Version { get; set; }

        public List<CatalogType> CatalogTypes { get; set; }
        public List<CatalogBrand> CatalogBrands { get; set; }
        public List<CatalogItem> CatalogItems { get; set; }
    }
}
