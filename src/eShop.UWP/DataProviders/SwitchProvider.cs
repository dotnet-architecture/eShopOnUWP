using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using eShop.UWP;
using eShop.Domain.Models;
using eShop.Providers.Contracts;

namespace eShop.Providers
{
    public class SwitchProvider : ICatalogProvider
    {
        public SwitchProvider()
        {
            LocalCatalogProvider = new LocalCatalogProvider();
            RESTCatalogProvider = new RESTCatalogProvider();
        }

        public ICatalogProvider LocalCatalogProvider { get; }
        public ICatalogProvider RESTCatalogProvider { get; }

        public ICatalogProvider CurrentProvider => AppSettings.Current.DataProvider == DataProviderType.Local ? LocalCatalogProvider : RESTCatalogProvider;

        public Task DeleteItemAsync(CatalogItem catalogItem)
        {
            return CurrentProvider.DeleteItemAsync(catalogItem);
        }

        public Task<IList<CatalogBrand>> GetCatalogBrandsAsync()
        {
            return CurrentProvider.GetCatalogBrandsAsync();
        }

        public Task<IList<CatalogType>> GetCatalogTypesAsync()
        {
            return CurrentProvider.GetCatalogTypesAsync();
        }

        public Task<CatalogItem> GetItemByIdAsync(int id)
        {
            return CurrentProvider.GetItemByIdAsync(id);
        }

        public Task<IList<CatalogItem>> GetItemsAsync(CatalogType selectedCatalogType, CatalogBrand selectedCatalogBrand, string query)
        {
            return CurrentProvider.GetItemsAsync(selectedCatalogType, selectedCatalogBrand, query);
        }

        public Task<IList<CatalogItem>> GetItemsByVoiceCommandAsync(string query)
        {
            return CurrentProvider.GetItemsByVoiceCommandAsync(query);
        }

        public Task<IList<CatalogItem>> RelatedItemsByTypeAsync(int catalogTypeId)
        {
            return CurrentProvider.RelatedItemsByTypeAsync(catalogTypeId);
        }

        public Task SaveItemAsync(CatalogItem item)
        {
            return CurrentProvider.SaveItemAsync(item);
        }
    }
}
