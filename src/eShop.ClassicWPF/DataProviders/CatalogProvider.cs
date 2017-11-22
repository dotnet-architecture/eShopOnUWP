using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using eShop.WPF;
using eShop.Models;

namespace eShop.Providers
{
    public partial class CatalogProvider
    {
        static public Result IsCurrentProviderAvailable()
        {
            var provider = new CatalogProvider();
            return provider.IsAvailable();
        }

        public CatalogProvider()
        {
            SqlCatalogProvider = new SqlCatalogProvider();
        }

        public string Name => Current.Name;

        public SqlCatalogProvider SqlCatalogProvider { get; }

        public SqlCatalogProvider Current => SqlCatalogProvider;

        public Result IsAvailable()
        {
            return Current.IsAvailable();
        }

        public IList<CatalogTypeModel> GetCatalogTypes()
        {
            try
            {
                CatalogTypes = Current.GetCatalogTypes();
                return CatalogTypes;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return new List<CatalogTypeModel>();
            }
        }

        public IList<CatalogBrandModel> GetCatalogBrands()
        {
            try
            {
                CatalogBrands = Current.GetCatalogBrands();
                return CatalogBrands;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return new List<CatalogBrandModel>();
            }
        }

        public CatalogItemModel GetItemById(int id)
        {
            try
            {
                return Current.GetItemById(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public IList<CatalogItemModel> GetItems(int typeId, int brandId, string query)
        {
            try
            {
                return Current.GetItems(typeId, brandId, query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return new List<CatalogItemModel>();
            }
        }

        public IList<CatalogItemModel> GetItemsByVoiceCommand(string query)
        {
            try
            {
                return Current.GetItemsByVoiceCommand(query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return new List<CatalogItemModel>();
            }
        }

        public IList<CatalogItemModel> RelatedItemsByType(int catalogTypeId)
        {
            try
            {
                return Current.RelatedItemsByType(catalogTypeId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return new List<CatalogItemModel>();
            }
        }

        public void SaveItem(CatalogItemModel item)
        {
            Current.SaveItem(item);
        }

        public void DeleteItem(CatalogItemModel catalogItem)
        {
            Current.DeleteItem(catalogItem);
        }
    }
}
