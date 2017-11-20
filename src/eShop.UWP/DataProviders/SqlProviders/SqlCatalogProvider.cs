using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Windows.Storage;

using eShop.UWP;
using eShop.Data;
using eShop.UWP.Models;
using eShop.SqlProvider;

namespace eShop.Providers
{
    public partial class SqlCatalogProvider : ICatalogProvider
    {
        static private IList<CatalogTypeModel> _catalogTypes = null;
        static private IList<CatalogBrandModel> _catalogBrands = null;

        public string Name => "SQL";

        public string ConnectionString => AppSettings.Current.SqlConnectionString;

        public Task<Result> IsAvailableAsync()
        {
            return Task<Result>.Run(() =>
            {
                try
                {
                    var provider = new SqlServerProvider(ConnectionString);
                    if (provider.DatabaseExists())
                    {
                        if (provider.IsLastVersion())
                        {
                            return Result.Ok();
                        }
                        return Result.Error("Version mismatch", "Database version mismatch. Please, press the 'Create' button and try again.");
                    }
                    return Result.Error("Database not found", "Database not found using current connection string. Please, press the 'Create' button and try again.");
                }
                catch (Exception ex)
                {
                    return Result.Error(ex);
                }
            });
        }

        public Task<IList<CatalogTypeModel>> GetCatalogTypesAsync()
        {
            return Task<IList<CatalogTypeModel>>.Run(() =>
            {
                _catalogTypes = new List<CatalogTypeModel>();
                var provider = new SqlServerProvider(ConnectionString);
                var dataTable = provider.GetCatalogTypes();
                foreach (DataRow item in dataTable.Rows)
                {
                    var record = new CatalogType
                    {
                        Id = (int)item["Id"],
                        Type = item["Name"] as String,
                    };
                    _catalogTypes.Add(new CatalogTypeModel(record));
                }
                return _catalogTypes;
            });
        }

        public Task<IList<CatalogBrandModel>> GetCatalogBrandsAsync()
        {
            return Task<IList<CatalogBrandModel>>.Run(() =>
            {
                _catalogBrands = new List<CatalogBrandModel>();
                var provider = new SqlServerProvider(ConnectionString);
                var dataTable = provider.GetCatalogBrands();
                foreach (DataRow item in dataTable.Rows)
                {
                    var record = new CatalogBrand
                    {
                        Id = (int)item["Id"],
                        Brand = item["Name"] as String,
                    };
                    _catalogBrands.Add(new CatalogBrandModel(record));
                }
                return _catalogBrands;
            });
        }

        public async Task<CatalogItemModel> GetItemByIdAsync(int id)
        {
            var provider = new SqlServerProvider(ConnectionString);
            var dataTable = provider.GetCatalogItem(id);
            if (dataTable.Rows.Count > 0)
            {
                var dataRow = dataTable.Rows[0];
                var item = CreateCatalogItem(dataRow);
                var model = new CatalogItemModel(item);
                await PopulateImageAsync(model, item.PictureFileName);
                return model;
            }
            return null;
        }

        public async Task<IList<CatalogItemModel>> GetItemsAsync(int typeId, int brandId, string query)
        {
            var records = new List<CatalogItemModel>();

            var provider = new SqlServerProvider(ConnectionString);
            var dataTable = provider.GetCatalogItemsFilter(typeId, brandId, query);
            foreach (DataRow dataRow in dataTable.Rows)
            {
                var item = CreateCatalogItem(dataRow);
                var model = new CatalogItemModel(item);
                await PopulateImageAsync(model, item.PictureFileName);
                records.Add(model);
            }

            return records.OrderBy(r => r.Name).ToList();
        }

        public async Task<IList<CatalogItemModel>> GetItemsByVoiceCommandAsync(string query)
        {
            var catalogTypes = await GetCatalogTypesAsync();
            var catalogBrands = await GetCatalogBrandsAsync();

            IEnumerable<CatalogItemModel> items = await GetItemsAsync(-1, -1, null);

            var queryIgnoreUpper = query?.ToUpperInvariant() ?? string.Empty;

            var filterType = catalogTypes.FirstOrDefault(item => item.Name.ToUpperInvariant().Contains(queryIgnoreUpper));
            if (filterType != null)
            {
                items = items.Where(item => item.Id == filterType.Id);
            }

            var filterBrand = catalogBrands.FirstOrDefault(item => item.Name.ToUpperInvariant().Contains(queryIgnoreUpper));
            if (filterBrand != null)
            {
                items = items.Where(item => item.Id == filterBrand.Id);
            }

            return items.ToList();
        }

        public Task<IList<CatalogItemModel>> RelatedItemsByTypeAsync(int catalogTypeId)
        {
            return GetItemsAsync(catalogTypeId, -1, null);
        }

        public Task SaveItemAsync(CatalogItemModel model)
        {
            return Task.Run(() =>
            {
                var item = model.Source;

                if (model.Picture != null)
                {
                    string extension = ContentTypes.GetExtensionFromContentType(model.PictureContentType);
                    item.PictureFileName = $"{DateTime.UtcNow.Ticks}{extension}";
                }

                var provider = new SqlServerProvider(ConnectionString);
                if (item.Id == 0)
                {
                    // New
                    item.Id = GenerateId();
                    provider.InsertCatalogItem(item.Id, item.Name, item.Description, item.PictureFileName, item.Price, item.CatalogTypeId, item.CatalogBrandId, item.IsDisabled);
                    provider.InsertCatalogImage(item.Id, model.Picture);
                }
                else
                {
                    // Update
                    if (model.Picture != null)
                    {
                        provider.UpdateCatalogImage(item.Id, model.Picture);
                    }
                    provider.UpdateCatalogItem(item.Id, item.Name, item.Description, item.PictureFileName, item.Price, item.CatalogTypeId, item.CatalogBrandId, item.IsDisabled);
                }
            });
        }

        public Task DeleteItemAsync(CatalogItemModel model)
        {
            return Task.Run(() =>
            {
                var provider = new SqlServerProvider(ConnectionString);
                provider.DeleteCatalogItem(model.Id);
            });
        }

        private CatalogItem CreateCatalogItem(DataRow dataRow)
        {
            return new CatalogItem
            {
                Id = (int)dataRow["Id"],
                Name = dataRow["Name"] as String,
                Description = dataRow["Description"] as String,
                PictureFileName = dataRow["PictureName"] as String,
                Price = (double)dataRow["Price"],
                CatalogTypeId = (int)dataRow["CatalogTypeId"],
                CatalogBrandId = (int)dataRow["CatalogBrandId"],
                IsDisabled = (bool)dataRow["IsDisabled"],
                LatUpdate = (DateTime)dataRow["LastUpdate"],
            };
        }

        private async Task PopulateImageAsync(CatalogItemModel model, string pictureFileName)
        {
            // TODO: Default picture
            model.PictureUri = null;

            if (pictureFileName != null)
            {
                var storage = new FileStorage(ApplicationData.Current.TemporaryFolder);
                if (await storage.FileExsits(pictureFileName))
                {
                    model.PictureUri = GetPictureUri(pictureFileName);
                }
                else
                {
                    var provider = new SqlServerProvider(ConnectionString);
                    var dataTable = provider.GetCatalogImage(model.Id);
                    if (dataTable.Rows.Count > 0)
                    {
                        var dataRow = dataTable.Rows[0];
                        var picture = dataRow["Picture"] as byte[];
                        if (picture != null)
                        {
                            await storage.WriteBytes(pictureFileName, picture);
                            model.PictureUri = GetPictureUri(pictureFileName);
                        }
                    }
                }
            }
        }

        private string GetPictureUri(string fileName)
        {
            return $"ms-appdata:///temp/{fileName}";
        }

        private int GenerateId()
        {
            // Generate a pseudo unique Id for demo purposes
            var secs = (DateTime.UtcNow - DateTime.Parse($"{DateTime.UtcNow.Year}/01/01")).TotalSeconds;
            return (int)((DateTime.UtcNow.Year - 2016) * 100000000 + secs);
        }
    }
}
