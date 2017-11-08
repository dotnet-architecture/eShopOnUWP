using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using eShop.UWP;
using eShop.UWP.Data;
using eShop.UWP.Models;
using eShop.SqlProvider;
using Windows.Storage;

namespace eShop.Providers
{
    public partial class SqlCatalogProvider : ICatalogProvider
    {
        static private IList<CatalogTypeModel> _catalogTypes = null;
        static private IList<CatalogBrandModel> _catalogBrands = null;

        public string ConnectionString => AppSettings.Current.SqlConnectionString;

        public Task<Result> IsAvailableAsync()
        {
            return Task<Result>.Run(() =>
            {
                try
                {
                    var provider = new SqlServerProvider(ConnectionString);
                    provider.GetCatalogTypes();
                    return Result.Ok();
                }
                catch (Exception ex)
                {
                    return Result.Error(ex);
                }
            });
        }

        public async Task<IList<CatalogTypeModel>> GetCatalogTypesAsync()
        {
            _catalogTypes = new List<CatalogTypeModel>();

            await Task.FromResult(true);
            var provider = new SqlServerProvider(ConnectionString);
            var dataSet = provider.GetCatalogTypes();

            foreach (DataRow item in dataSet.Tables["CatalogTypes"].Rows)
            {
                var record = new CatalogType
                {
                    Id = (int)item["Id"],
                    Type = item["Type"] as String,
                };
                _catalogTypes.Add(new CatalogTypeModel(record));
            }

            return _catalogTypes;
        }

        public async Task<IList<CatalogBrandModel>> GetCatalogBrandsAsync()
        {
            _catalogBrands = new List<CatalogBrandModel>();

            await Task.FromResult(true);
            var provider = new SqlServerProvider(ConnectionString);
            var dataSet = provider.GetCatalogBrands();

            foreach (DataRow item in dataSet.Tables["CatalogBrands"].Rows)
            {
                var record = new CatalogBrand
                {
                    Id = (int)item["Id"],
                    Brand = item["Brand"] as String,
                };
                _catalogBrands.Add(new CatalogBrandModel(record));
            }

            return _catalogBrands;
        }

        public async Task<CatalogItemModel> GetItemByIdAsync(int id)
        {
            await Task.FromResult(true);

            var provider = new SqlServerProvider(ConnectionString);
            var dataSet = provider.GetItemById(id);
            var dataTable = dataSet.Tables["CatalogItems"];

            if (dataTable.Rows.Count > 0)
            {
                var dataRow = dataTable.Rows[0];
                var item = CreateCatalogItem(dataRow);
                await PopulateImageAsync(item);
                return item;
            }

            return null;
        }

        public async Task<IList<CatalogItemModel>> GetItemsAsync(int typeId, int brandId, string query)
        {
            List<CatalogItemModel> records = new List<CatalogItemModel>();

            await Task.FromResult(true);

            var provider = new SqlServerProvider(ConnectionString);
            var dataSet = provider.GetItems(typeId, brandId, query);
            var dataTable = dataSet.Tables["CatalogItems"];
            foreach (DataRow dataRow in dataTable.Rows)
            {
                var item = CreateCatalogItem(dataRow);
                await PopulateImageAsync(item);
                records.Add(item);
            }

            return records.OrderBy(r => r.Name).ToList();
        }

        public async Task<IList<CatalogItemModel>> GetItemsByVoiceCommandAsync(string query)
        {
            await Task.FromResult(true);
            // TODO: 
            throw new NotImplementedException();
        }

        public Task<IList<CatalogItemModel>> RelatedItemsByTypeAsync(int catalogTypeId)
        {
            return GetItemsAsync(catalogTypeId, -1, null);
        }

        public async Task SaveItemAsync(CatalogItemModel model)
        {
            await Task.FromResult(true);
            var provider = new SqlServerProvider(ConnectionString);
            var dataSet = provider.GetDatasetSchema();
            var dataTable = dataSet.Tables["CatalogItems"];
            var item = model.Source;
            dataTable.Rows.Add(0, item.Name, item.Description, item.Price, item.CatalogTypeId, item.CatalogBrandId);
            provider.CreateCatalogItems(dataSet);
        }

        public async Task DeleteItemAsync(CatalogItemModel model)
        {
            await Task.FromResult(true);
            var provider = new SqlServerProvider(ConnectionString);
            provider.DeleteItem(model.Id);
        }

        private CatalogItemModel CreateCatalogItem(DataRow dataRow)
        {
            int id = (int)dataRow["Id"];
            int typeId = (int)dataRow["CatalogTypeId"];
            int brandId = (int)dataRow["CatalogBrandId"];

            var record = new CatalogItem
            {
                Id = id,
                Price = (double)dataRow["Price"],
                Name = dataRow["Name"] as String,
                Description = dataRow["Description"] as String,
                CatalogTypeId = typeId,
                CatalogBrandId = brandId
            };
            return new CatalogItemModel(record);
        }

        private async Task PopulateImageAsync(CatalogItemModel item)
        {
            var provider = new SqlServerProvider(ConnectionString);
            var dataSet = provider.GetImage(item.Id);
            var dataTable = dataSet.Tables["CatalogImages"];

            if (dataTable.Rows.Count > 0)
            {
                var dataRow = dataTable.Rows[0];
                var bytes = dataRow["ImageBytes"] as byte[];
                if (bytes != null)
                {
                    string extension = (dataRow["ImageType"] as String) ?? ".jpg";
                    string fileName = $"{item.Id}{extension}";
                    await SaveTempImageAsync(fileName, bytes);
                    item.PictureUri = $"ms-appdata:///temp/{fileName}";
                }
            }
        }

        private async Task SaveTempImageAsync(string fileName, byte[] bytes)
        {
            try
            {
                var folder = ApplicationData.Current.TemporaryFolder;
                var storageFile = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                using (var randomStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (var stream = new BinaryWriter(randomStream.AsStreamForWrite()))
                    {
                        stream.Write(bytes);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
