using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using eShop.UWP;
using eShop.Data;
using eShop.UWP.Models;
using eShop.UWP.Services;

namespace eShop.Providers
{
    public class RESTCatalogProvider : ICatalogProvider
    {
        private IList<CatalogTypeModel> _catalogTypes = null;
        private IList<CatalogBrandModel> _catalogBrands = null;

        public string Name => "REST";

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

        public async Task<IList<CatalogTypeModel>> GetCatalogTypesAsync()
        {
            if (_catalogTypes == null)
            {
                using (var cli = new WebApiClient(BaseAddressUri))
                {
                    var records = (await cli.GetAsync<IEnumerable<CatalogType>>("api/v1/catalog/CatalogTypes"));
                    _catalogTypes = records.Select(r => new CatalogTypeModel(r)).ToList();
                }
            }
            return _catalogTypes;
        }

        public async Task<IList<CatalogBrandModel>> GetCatalogBrandsAsync()
        {
            if (_catalogBrands == null)
            {
                using (var cli = new WebApiClient(BaseAddressUri))
                {
                    var records = (await cli.GetAsync<IEnumerable<CatalogBrand>>("api/v1/catalog/CatalogBrands"));
                    _catalogBrands = records.Select(r => new CatalogBrandModel(r)).ToList();
                }
            }
            return _catalogBrands;
        }


        public async Task<CatalogItemModel> GetItemByIdAsync(int id)
        {
            using (var cli = new WebApiClient(BaseAddressUri))
            {
                var record = await cli.GetAsync<CatalogItem>($"api/v1/catalog/items/{id}");
                return new CatalogItemModel(record);
            }
        }

        public async Task<IList<CatalogItemModel>> GetItemsAsync(int typeId, int brandId, string query)
        {
            string path = $"api/v1/catalog/items/type/{typeId}/brand/{brandId}";

            using (var cli = new WebApiClient(BaseAddressUri))
            {
                var pagination = await cli.GetAsync<PaginatedItems<CatalogItem>>(path, QueryParam.Create("pageSize", 100));
                var records = pagination.Data;

                if (!String.IsNullOrEmpty(query))
                {
                    records = records.Where(r => $"{r.Name}".ToUpper().Contains(query.ToUpper()));
                }

                return records.Select(r => new CatalogItemModel(r)).ToList();
            }
        }

        public async Task<IList<CatalogItemModel>> RelatedItemsByTypeAsync(int catalogTypeId)
        {
            using (var cli = new WebApiClient(BaseAddressUri))
            {
                var pagination = await cli.GetAsync<PaginatedItems<CatalogItem>>("api/v1/catalog/items", QueryParam.Create("pageSize", 100));
                var records = pagination.Data;
                records = records.Where(r => r.CatalogTypeId == catalogTypeId);
                return records.Select(r => new CatalogItemModel(r)).ToList();
            }
        }

        public async Task<IList<CatalogItemModel>> GetItemsByVoiceCommandAsync(string query)
        {
            using (var cli = new WebApiClient(BaseAddressUri))
            {
                var pagination = await cli.GetAsync<PaginatedItems<CatalogItem>>("api/v1/catalog/items", QueryParam.Create("pageSize", 100));
                var records = pagination.Data;

                var queryIgnoreUpper = query?.ToUpperInvariant() ?? string.Empty;

                var filterType = (await GetCatalogTypesAsync()).FirstOrDefault(item => item.Name.ToUpperInvariant().Contains(queryIgnoreUpper));
                if (filterType != null)
                {
                    records = records.Where(item => item.CatalogTypeId == filterType.Id);
                }

                var filterBrand = (await GetCatalogBrandsAsync()).FirstOrDefault(item => item.Name.ToUpperInvariant().Contains(queryIgnoreUpper));
                if (filterBrand != null)
                {
                    records = records.Where(item => item.CatalogBrandId == filterBrand.Id);
                }

                return records.Select(r => new CatalogItemModel(r)).ToList();
            }
        }

        public async Task SaveItemAsync(CatalogItemModel item)
        {
            var picture = item.Picture;
            var contentType = item.PictureContentType;
            item.Picture = null;

            using (var cli = new WebApiClient(BaseAddressUri))
            {
                if (item.Id == 0)
                {
                    // Create (POST)
                    var record = await cli.PostAsync<CatalogItem>("api/v1/catalog/items", item.Source);
                    item = new CatalogItemModel(record);
                }
                else
                {
                    // Update (PUT)
                    await cli.PutAsync<string>("api/v1/catalog/items", item.Source);
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

        public async Task DeleteItemAsync(CatalogItemModel item)
        {
            using (var cli = new WebApiClient(BaseAddressUri))
            {
                await cli.DeleteAsync($"api/v1/catalog/{item.Id}");
            }
        }

        static public string BaseAddressUri
        {
            get { return AppSettings.Current.ServiceUrl; }
        }
    }
}
