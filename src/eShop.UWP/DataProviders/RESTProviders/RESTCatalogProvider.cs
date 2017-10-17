using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using eShop.Domain.Models;
using eShop.Providers.Contracts;
using eShop.UWP.Services;

namespace eShop.Providers
{
    public class RESTCatalogProvider : ICatalogProvider
    {
        private IList<CatalogType> _catalogTypes = null;
        private IList<CatalogBrand> _catalogBrands = null;

        public async Task<IList<CatalogType>> GetCatalogTypesAsync()
        {
            if (_catalogTypes == null)
            {
                using (var cli = new WebApiClient(BaseAddressUri))
                {
                    _catalogTypes = (await cli.GetAsync<IEnumerable<CatalogType>>("api/v1/catalog/CatalogTypes")).ToList();
                }
            }
            return _catalogTypes;
        }

        public async Task<IList<CatalogBrand>> GetCatalogBrandsAsync()
        {
            if (_catalogBrands == null)
            {
                using (var cli = new WebApiClient(BaseAddressUri))
                {
                    _catalogBrands = (await cli.GetAsync<IEnumerable<CatalogBrand>>("api/v1/catalog/CatalogBrands")).ToList();
                }
            }
            return _catalogBrands;
        }

        public async Task<CatalogItem> GetItemByIdAsync(int id)
        {
            using (var cli = new WebApiClient(BaseAddressUri))
            {
                var item = await cli.GetAsync<CatalogItem>($"api/v1/catalog/items/{id}");
                await Populate(item);
                return item;
            }
        }

        public async Task<IList<CatalogItem>> GetItemsAsync(CatalogType selectedCatalogType, CatalogBrand selectedCatalogBrand, string query)
        {
            using (var cli = new WebApiClient(BaseAddressUri))
            {
                var pagination = await cli.GetAsync<PaginatedItems<CatalogItem>>("api/v1/catalog/items", QueryParam.Create("pageSize", 100));
                var items = pagination.Data;
                await Populate(items);
                return items.ToList();
            }
        }

        public async Task<IList<CatalogItem>> RelatedItemsByTypeAsync(int catalogTypeId)
        {
            using (var cli = new WebApiClient(BaseAddressUri))
            {
                var pagination = await cli.GetAsync<PaginatedItems<CatalogItem>>("api/v1/catalog/items", QueryParam.Create("pageSize", 100));
                var items = pagination.Data;
                items = items.Where(r => r.CatalogTypeId == catalogTypeId);
                await Populate(items);
                return items.ToList();
            }
        }

        public async Task<IList<CatalogItem>> GetItemsByVoiceCommandAsync(string query)
        {
            using (var cli = new WebApiClient(BaseAddressUri))
            {
                var pagination = await cli.GetAsync<PaginatedItems<CatalogItem>>("api/v1/catalog/items", QueryParam.Create("pageSize", 100));
                var items = pagination.Data;

                var queryIgnoreUpper = query?.ToUpperInvariant() ?? string.Empty;

                var filterType = (await GetCatalogTypesAsync()).FirstOrDefault(item => item.Type.ToUpperInvariant().Contains(queryIgnoreUpper));
                if (filterType != null)
                {
                    items = items.Where(item => item.CatalogType.Id == filterType.Id);
                }

                var filterBrand = (await GetCatalogBrandsAsync()).FirstOrDefault(item => item.Brand.ToUpperInvariant().Contains(queryIgnoreUpper));
                if (filterBrand != null)
                {
                    items = items.Where(item => item.CatalogBrand.Id == filterBrand.Id);
                }

                return (await Populate(items.ToArray())).OrderBy(r => r.Name).ToList();
            }
        }

        public async Task SaveItemAsync(CatalogItem item)
        {
            using (var cli = new WebApiClient(BaseAddressUri))
            {
                var oldItem = await GetItemByIdAsync(item.Id);
                if (oldItem == null)
                {
                    // Create (POST)
                    await cli.PostAsync<string>("api/v1/catalog/items", item);
                }
                else
                {
                    // Update (PUT)
                    await cli.PutAsync<string>("api/v1/catalog/items", item);
                }
            }
        }

        public async Task DeleteItemAsync(CatalogItem item)
        {
            using (var cli = new WebApiClient(BaseAddressUri))
            {
                await cli.DeleteAsync($"api/v1/catalog/items/{item.Id}");
            }
        }

        private async Task<IEnumerable<CatalogItem>> Populate(IEnumerable<CatalogItem> items)
        {
            foreach (var item in items)
            {
                await Populate(item);
            }
            return items;
        }

        private async Task<CatalogItem> Populate(CatalogItem item)
        {
            if (item != null)
            {
                item.CatalogType = (await GetCatalogTypesAsync()).FirstOrDefault(r => r.Id == item.CatalogTypeId);
                item.CatalogBrand = (await GetCatalogBrandsAsync()).FirstOrDefault(r => r.Id == item.CatalogBrandId);
            }
            return item;
        }

        static public string BaseAddressUri
        {
            get { return "http://localhost:5101/"; }
        }
    }
}
