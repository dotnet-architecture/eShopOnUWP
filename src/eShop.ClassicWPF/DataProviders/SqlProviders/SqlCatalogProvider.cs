using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Collections.Generic;

using eShop.WPF;
using eShop.Data;
using eShop.Models;
using eShop.SqlProvider;

namespace eShop.Providers
{
    public partial class SqlCatalogProvider
    {
        static private IList<CatalogTypeModel> _catalogTypes = null;
        static private IList<CatalogBrandModel> _catalogBrands = null;

        public string Name => "SQL";

        public string ConnectionString => AppSettings.Current.SqlConnectionString;

        public Result IsAvailable()
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
        }

        public IList<CatalogTypeModel> GetCatalogTypes()
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
        }

        public IList<CatalogBrandModel> GetCatalogBrands()
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
        }

        public CatalogItemModel GetItemById(int id)
        {
            var provider = new SqlServerProvider(ConnectionString);
            var dataTable = provider.GetCatalogItem(id);
            if (dataTable.Rows.Count > 0)
            {
                var dataRow = dataTable.Rows[0];
                var item = CreateCatalogItem(dataRow);
                var model = new CatalogItemModel(item);
                PopulateImage(model, item.PictureFileName);
                return model;
            }
            return null;
        }

        public IList<CatalogItemModel> GetItems(int typeId, int brandId, string query)
        {
            var records = new List<CatalogItemModel>();

            var provider = new SqlServerProvider(ConnectionString);
            var dataTable = provider.GetCatalogItemsFilter(typeId, brandId, query);
            foreach (DataRow dataRow in dataTable.Rows)
            {
                var item = CreateCatalogItem(dataRow);
                var model = new CatalogItemModel(item);
                PopulateImage(model, item.PictureFileName);
                records.Add(model);
            }

            return records.OrderBy(r => r.Name).ToList();
        }

        public IList<CatalogItemModel> GetItemsByVoiceCommand(string query)
        {
            throw new NotImplementedException();
        }

        public IList<CatalogItemModel> RelatedItemsByType(int catalogTypeId)
        {
            return GetItems(catalogTypeId, -1, null);
        }

        public void SaveItem(CatalogItemModel model)
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
        }

        public void DeleteItem(CatalogItemModel model)
        {
            var provider = new SqlServerProvider(ConnectionString);
            provider.DeleteCatalogItem(model.Id);
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

        private void PopulateImage(CatalogItemModel model, string pictureFileName)
        {
            // TODO: Default picture
            model.PictureUri = null;

            if (pictureFileName != null)
            {
                var storage = new FileStorage("Images");
                if (storage.FileExsits(pictureFileName))
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
                            storage.WriteBytes(pictureFileName, picture);
                            model.PictureUri = GetPictureUri(pictureFileName);
                        }
                    }
                }
            }
        }

        private string GetPictureUri(string fileName)
        {
            return Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);
        }

        private int GenerateId()
        {
            // Generate a pseudo unique Id for demo purposes
            var secs = (DateTime.UtcNow - DateTime.Parse($"{DateTime.UtcNow.Year}/01/01")).TotalSeconds;
            return (int)((DateTime.UtcNow.Year - 2016) * 100000000 + secs);
        }
    }
}
