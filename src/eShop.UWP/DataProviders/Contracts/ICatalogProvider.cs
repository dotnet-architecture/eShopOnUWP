using System.Collections.Generic;
using eShop.Domain.Models;

namespace eShop.Providers.Contracts
{
    public interface ICatalogProvider
    {
        IList<CatalogBrand> GetCatalogBrands();
        IList<CatalogType> GetCatalogTypes();
        IList<CatalogItem> GetItems(CatalogType selectedCatalogType, CatalogBrand selectedCatalogBrand, string query);
        IList<CatalogItem> GetItemsByVoiceCommand(string query);
        CatalogItem GetItemById(int id);
        IList<CatalogItem> RelatedItemsByType(int catalogTypeId);
        void SaveItem(CatalogItem item);
        void DeleteItem(CatalogItem catalogItem);
    }
}
