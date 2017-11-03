using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using eShop.Domain.Models;
using eShop.Providers.Contracts;
using eShop.SqlProvider;
using eShop.UWP;

namespace eShop.Providers
{
    public partial class SqlCatalogProvider : ICatalogProvider
    {
        static private IList<CatalogType> _catalogTypes = null;
        static private IList<CatalogBrand> _catalogBrands = null;

        private string _connectionString = null;
        public string ConnectionString
        {
            get => _connectionString ?? (_connectionString = AppSettings.Current.SqlConnectionString);
            set => _connectionString = value;
        }

        public Task<Result> IsAvailableAsync()
        {
            return Task<Result>.Run(() =>
            {
                try
                {
                    var provider = new CatalogProvider(ConnectionString);
                    provider.GetCatalogTypes();
                    return Result.Ok();
                }
                catch (Exception ex)
                {
                    return Result.Error(ex);
                }
            });
        }

        public async Task<IList<CatalogType>> GetCatalogTypesAsync()
        {
            _catalogTypes = new List<CatalogType>();

            await Task.FromResult(true);
            var provider = new CatalogProvider(ConnectionString);
            var dataSet = provider.GetCatalogTypes();

            foreach (DataRow item in dataSet.Tables["CatalogTypes"].Rows)
            {
                _catalogTypes.Add(new CatalogType
                {
                    Id = (int)item["Id"],
                    Type = item["Type"] as String,
                });
            }

            return _catalogTypes;
        }

        public async Task<IList<CatalogBrand>> GetCatalogBrandsAsync()
        {
            _catalogBrands = new List<CatalogBrand>();

            await Task.FromResult(true);
            var provider = new CatalogProvider(ConnectionString);
            var dataSet = provider.GetCatalogBrands();

            foreach (DataRow item in dataSet.Tables["CatalogBrands"].Rows)
            {
                _catalogBrands.Add(new CatalogBrand
                {
                    Id = (int)item["Id"],
                    Brand = item["Brand"] as String,
                });
            }

            return _catalogBrands;
        }

        public async Task<CatalogItem> GetItemByIdAsync(int id)
        {
            await Task.FromResult(true);

            var provider = new CatalogProvider(ConnectionString);
            var dataSet = provider.GetItemById(id);
            var dataTable = dataSet.Tables["CatalogItems"];

            if (dataTable.Rows.Count > 0)
            {
                var dataRow = dataTable.Rows[0];
                return CreateCatalogItem(dataRow);
            }

            return null;
        }

        public async Task<IList<CatalogItem>> GetItemsAsync(CatalogType catalogType, CatalogBrand catalogBrand, string query)
        {
            List<CatalogItem> records = new List<CatalogItem>();

            int typeId = catalogType == null ? -1 : catalogType.Id;
            int brandId = catalogBrand == null ? -1 : catalogBrand.Id;

            await Task.FromResult(true);

            var provider = new CatalogProvider(ConnectionString);
            var dataSet = provider.GetItems(typeId, brandId, query);
            var dataTable = dataSet.Tables["CatalogItems"];
            foreach (DataRow dataRow in dataTable.Rows)
            {
                records.Add(CreateCatalogItem(dataRow));
            }

            return records.OrderBy(r => r.Name).ToList();
        }

        public async Task<IList<CatalogItem>> GetItemsByVoiceCommandAsync(string query)
        {
            await Task.FromResult(true);
            // TODO: 
            throw new NotImplementedException();
        }

        public Task<IList<CatalogItem>> RelatedItemsByTypeAsync(int catalogTypeId)
        {
            return GetItemsAsync(new CatalogType { Id = catalogTypeId }, null, null);
        }

        public async Task SaveItemAsync(CatalogItem item)
        {
            await Task.FromResult(true);
            var provider = new CatalogProvider(ConnectionString);
            var dataSet = provider.GetDatasetSchema();
            var dataTable = dataSet.Tables["CatalogItems"];
            dataTable.Rows.Add(0, item.Name, item.Description, item.Price, item.CatalogTypeId, item.CatalogBrandId);
            provider.CreateCatalogItems(dataSet);
        }

        public async Task DeleteItemAsync(CatalogItem item)
        {
            await Task.FromResult(true);
            var provider = new CatalogProvider(ConnectionString);
            provider.DeleteItem(item.Id);
        }

        private CatalogItem CreateCatalogItem(DataRow dataRow)
        {
            int id = (int)dataRow["Id"];
            int typeId = (int)dataRow["CatalogTypeId"];
            int brandId = (int)dataRow["CatalogBrandId"];

            return new CatalogItem
            {
                Id = id,
                Price = (double)dataRow["Price"],
                Name = dataRow["Name"] as String,
                Description = dataRow["Description"] as String,
                CatalogTypeId = typeId,
                CatalogType = _catalogTypes.Where(r => r.Id == typeId).FirstOrDefault(),
                CatalogBrandId = brandId,
                CatalogBrand = _catalogBrands.Where(r => r.Id == brandId).FirstOrDefault(),
                PictureUri = $"ms-appx:///Assets/Images/Catalog/{id}.png"
            };
        }
    }
}
