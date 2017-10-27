using System;
using System.IO;
using System.Collections.Generic;

using eShop.UWP.Services;
using eShop.Domain.Models;
using System.Reflection;
using eShop.UWP.Helpers;

namespace eShop.Providers
{
    public class LocalCatalogDb : JsonDb
    {
        public LocalCatalogDb() : base("LocalCatalogDb.json")
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
