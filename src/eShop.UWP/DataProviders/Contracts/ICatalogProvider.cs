using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using eShop.Domain.Models;
using eShop.UWP;

namespace eShop.Providers.Contracts
{
    public interface ICatalogProvider
    {
        Task<Result> IsAvailableAsync();

        Task<IList<CatalogType>> GetCatalogTypesAsync();
        Task<IList<CatalogBrand>> GetCatalogBrandsAsync();

        Task<CatalogItem> GetItemByIdAsync(int id);
        Task<IList<CatalogItem>> GetItemsAsync(CatalogType catalogType, CatalogBrand catalogBrand, string query);
        Task<IList<CatalogItem>> RelatedItemsByTypeAsync(int catalogTypeId);

        Task<IList<CatalogItem>> GetItemsByVoiceCommandAsync(string query);

        Task SaveItemAsync(CatalogItem item);
        Task DeleteItemAsync(CatalogItem catalogItem);
    }
}
