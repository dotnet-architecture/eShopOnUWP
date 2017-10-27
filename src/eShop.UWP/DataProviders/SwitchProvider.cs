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
        static public async Task<Result> IsCurrentProviderAvailableAsync()
        {
            var provider = new SwitchProvider();
            return await provider.IsAvailableAsync();
        }

        public SwitchProvider()
        {
            LocalCatalogProvider = new LocalCatalogProvider();
            RESTCatalogProvider = new RESTCatalogProvider();
        }

        public ICatalogProvider LocalCatalogProvider { get; }
        public ICatalogProvider RESTCatalogProvider { get; }

        public ICatalogProvider Current => AppSettings.Current.DataProvider == DataProviderType.Local ? LocalCatalogProvider : RESTCatalogProvider;

        public async Task<Result> IsAvailableAsync()
        {
            return await Current.IsAvailableAsync();
        }

        public async Task DeleteItemAsync(CatalogItem catalogItem)
        {
            try
            {
                await Current.DeleteItemAsync(catalogItem);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public async Task<IList<CatalogBrand>> GetCatalogBrandsAsync()
        {
            try
            {
                return await Current.GetCatalogBrandsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return new List<CatalogBrand>();
            }
        }

        public async Task<IList<CatalogType>> GetCatalogTypesAsync()
        {
            try
            {
                return await Current.GetCatalogTypesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return new List<CatalogType>();
            }
        }

        public async Task<CatalogItem> GetItemByIdAsync(int id)
        {
            try
            {
                return await Current.GetItemByIdAsync(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<IList<CatalogItem>> GetItemsAsync(CatalogType selectedCatalogType, CatalogBrand selectedCatalogBrand, string query)
        {
            try
            {
                return await Current.GetItemsAsync(selectedCatalogType, selectedCatalogBrand, query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return new List<CatalogItem>();
            }
        }

        public async Task<IList<CatalogItem>> GetItemsByVoiceCommandAsync(string query)
        {
            try
            {
                return await Current.GetItemsByVoiceCommandAsync(query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return new List<CatalogItem>();
            }
        }

        public async Task<IList<CatalogItem>> RelatedItemsByTypeAsync(int catalogTypeId)
        {
            try
            {
                return await Current.RelatedItemsByTypeAsync(catalogTypeId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return new List<CatalogItem>();
            }
        }

        public async Task SaveItemAsync(CatalogItem item)
        {
            try
            {
                await Current.SaveItemAsync(item);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
