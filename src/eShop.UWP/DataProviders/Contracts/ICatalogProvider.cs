using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using eShop.UWP;
using eShop.UWP.Models;

namespace eShop.Providers
{
    public interface ICatalogProvider
    {
        string Name { get; }

        Task<Result> IsAvailableAsync();

        Task<IList<CatalogTypeModel>> GetCatalogTypesAsync();
        Task<IList<CatalogBrandModel>> GetCatalogBrandsAsync();

        Task<CatalogItemModel> GetItemByIdAsync(int id);
        Task<IList<CatalogItemModel>> GetItemsAsync(int typeId, int brandId, string query);
        Task<IList<CatalogItemModel>> RelatedItemsByTypeAsync(int catalogTypeId);

        Task<IList<CatalogItemModel>> GetItemsByVoiceCommandAsync(string query);

        Task SaveItemAsync(CatalogItemModel item);
        Task DeleteItemAsync(CatalogItemModel catalogItem);
    }
}
