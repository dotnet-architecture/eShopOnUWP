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
    public class SqlCatalogProvider : ICatalogProvider
    {
        public Task<Result> IsAvailableAsync()
        {
            return Task<Result>.Run(() =>
            {
                try
                {
                    var provider = new CatalogProvider(AppSettings.Current.SqlConnectionString);
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
            List<CatalogType> records = new List<CatalogType>();

            await Task.FromResult(true);
            var provider = new CatalogProvider(AppSettings.Current.SqlConnectionString);
            var dataSet = provider.GetCatalogTypes();

            foreach (DataRow item in dataSet.Tables["CatalogTypes"].Rows)
            {
                records.Add(new CatalogType
                {
                    Id = (int)item["Id"],
                    Type = item["Brand"] as String,
                });
            }

            return records.OrderBy(r => r.Type).ToList();
        }

        public async Task<IList<CatalogBrand>> GetCatalogBrandsAsync()
        {
            List<CatalogBrand> records = new List<CatalogBrand>();

            await Task.FromResult(true);
            var provider = new CatalogProvider(AppSettings.Current.SqlConnectionString);
            var dataSet = provider.GetCatalogBrands();

            foreach (DataRow item in dataSet.Tables["CatalogBrands"].Rows)
            {
                records.Add(new CatalogBrand
                {
                    Id = (int)item["Id"],
                    Brand = item["Brand"] as String,
                });
            }

            return records.OrderBy(r => r.Brand).ToList();
        }

        public async Task<CatalogItem> GetItemByIdAsync(int id)
        {
            await Task.FromResult(true);

            var provider = new CatalogProvider(AppSettings.Current.SqlConnectionString);
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

            var provider = new CatalogProvider(AppSettings.Current.SqlConnectionString);
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
            // TODO: 
            throw new NotImplementedException();
        }

        public async Task DeleteItemAsync(CatalogItem item)
        {
            await Task.FromResult(true);
            // TODO: 
            throw new NotImplementedException();
        }

        private CatalogItem CreateCatalogItem(DataRow dataRow)
        {
            return new CatalogItem
            {
                Id = (int)dataRow["Id"],
                Price = (double)dataRow["Price"],
                Name = dataRow["Name"] as String,
                Description = dataRow["Description"] as String,
                CatalogBrandId = (int)dataRow["CatalogBrandId"],
                CatalogTypeId = (int)dataRow["CatalogTypeId"]
            };
        }
    }
}
