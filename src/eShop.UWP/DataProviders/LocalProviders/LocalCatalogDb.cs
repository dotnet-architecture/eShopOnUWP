using System;
using System.IO;
using System.Collections.Generic;

using eShop.UWP.Data;
using eShop.UWP.Helpers;
using eShop.UWP.Services;

namespace eShop.Providers
{
    public class LocalCatalogDb : JsonDb
    {
        public LocalCatalogDb(string fileName = "LocalCatalogDb.json") : base(fileName)
        {
            if (!File.Exists(base.FileName))
            {
                string json = Resources.LoadString("CatalogDb.CatalogDb.json");
                Deserialize(json);
                SaveChanges();
            }
        }

        public List<CatalogType> CatalogTypes { get; set; }
        public List<CatalogBrand> CatalogBrands { get; set; }
        public List<CatalogItem> CatalogItems { get; set; }
    }
}
