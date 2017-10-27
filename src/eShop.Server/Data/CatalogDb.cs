using System;
using System.Collections.Generic;

using eShop.Server;

namespace eShop.Data
{
    public class CatalogDb : JsonDb
    {
        const string CATALOG_PATH = "_Db\\Catalog.json";

        public CatalogDb() : base(CATALOG_PATH)
        {
        }

        public List<CatalogType> CatalogTypes { get; set; }
        public List<CatalogBrand> CatalogBrands { get; set; }
        public List<CatalogItem> CatalogItems { get; set; }
    }
}
