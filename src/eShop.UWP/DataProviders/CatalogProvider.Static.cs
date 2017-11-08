using System;
using System.Linq;
using System.Collections.Generic;

using eShop.UWP.Models;

namespace eShop.Providers
{
    partial class CatalogProvider
    {
        static public IList<CatalogTypeModel> CatalogTypes { get; private set; }
        static public IList<CatalogBrandModel> CatalogBrands { get; private set; }

        static CatalogProvider()
        {
            CatalogTypes = new List<CatalogTypeModel>();
            CatalogBrands = new List<CatalogBrandModel>();
        }

        static public CatalogTypeModel GetCatalogType(int id)
        {
            return CatalogTypes.Where(r => r.Id == id).FirstOrDefault();
        }

        static public CatalogBrandModel GetCatalogBrand(int id)
        {
            return CatalogBrands.Where(r => r.Id == id).FirstOrDefault();
        }
    }
}
