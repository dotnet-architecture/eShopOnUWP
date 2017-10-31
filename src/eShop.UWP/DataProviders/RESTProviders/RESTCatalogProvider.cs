using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using eShop.Domain.Models;
using eShop.Providers.Contracts;
using eShop.UWP.Services;
using eShop.UWP;

namespace eShop.Providers
{
    public class RESTCatalogProvider : ICatalogProvider
    {
        private IList<CatalogType> _catalogTypes = null;
        private IList<CatalogBrand> _catalogBrands = null;

        public async Task<Result> IsAvailableAsync()
        {
            try
            {
                using (var cli = new WebApiClient(BaseAddressUri))
                {
                    await cli.GetAsync<IEnumerable<CatalogType>>("api/v1/catalog/CatalogTypes");
                    return Result.Ok();
                }
            }
            catch (Exception ex)
            {
                return Result.Error(ex);
            }
        }

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

        public async Task<IList<CatalogItem>> GetItemsAsync(CatalogType catalogType, CatalogBrand catalogBrand, string query)
        {
            int catalogTypeId = catalogType == null ? 0 : catalogType.Id;
            int catalogBrandId = catalogBrand == null ? 0 : catalogBrand.Id;

            string path = $"api/v1/catalog/items/type/{catalogTypeId}/brand/{catalogBrandId}";

            using (var cli = new WebApiClient(BaseAddressUri))
            {
                var pagination = await cli.GetAsync<PaginatedItems<CatalogItem>>(path, QueryParam.Create("pageSize", 100));
                var items = pagination.Data;

                if (!String.IsNullOrEmpty(query))
                {
                    items = items.Where(r => $"{r.Name}".ToUpper().Contains(query.ToUpper()));
                }

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
            var picture = item.Picture;
            var contentType = item.PictureContentType;

            using (var cli = new WebApiClient(BaseAddressUri))
            {
                if (item.Id == 0)
                {
                    // Create (POST)
                    item = await cli.PostAsync<CatalogItem>("api/v1/catalog/items", item);
                }
                else
                {
                    // Update (PUT)
                    await cli.PutAsync<string>("api/v1/catalog/items", item);
                }

                if (picture != null)
                {
                    using (var stream = new MemoryStream(picture))
                    {
                        await cli.PutStreamAsync($"api/v1/catalog/items/{item.Id}/pic", stream, contentType);
                    }
                }
            }
        }

        public async Task DeleteItemAsync(CatalogItem item)
        {
            using (var cli = new WebApiClient(BaseAddressUri))
            {
                await cli.DeleteAsync($"api/v1/catalog/{item.Id}");
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
            get { return AppSettings.Current.ServiceUrl; }
        }
    }
}
