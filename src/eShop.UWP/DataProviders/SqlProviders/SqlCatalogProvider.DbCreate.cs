using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using eShop.UWP;
using eShop.SqlProvider;
using eShop.UWP.Data;
using Windows.Storage;
using System.Data;

namespace eShop.Providers
{
    public partial class SqlCatalogProvider : ICatalogProvider
    {
        static public Result<bool> DatabaseExists(string connectionString)
        {
            try
            {
                var provider = new SqlServerProvider(connectionString);
                return Result<bool>.Ok(provider.DatabaseExists());
            }
            catch (Exception ex)
            {
                return Result<bool>.Error(ex);
            }
        }

        static public async Task<Result> CreateDatabaseAsync(string connectionString)
        {
            return await Task.Run<Result>(() =>
            {
                try
                {
                    var provider = new SqlServerProvider(connectionString);
                    provider.CreateDatabase();
                    return Result.Ok();
                }
                catch (Exception ex)
                {
                    return Result.Error(ex);
                }
            });
        }

        static public async Task FillDatabase(string connectionString)
        {
            var provider = new SqlServerProvider(connectionString);
            using (var db = new LocalCatalogDb("TempCatalogDb.json"))
            {
                CreateCatalogTypes(provider, db.CatalogTypes);
                CreateCatalogBrands(provider, db.CatalogBrands);
                var dataTableItems = CreateCatalogItems(provider, db.CatalogItems);
                await CreateCatalogImages(provider, dataTableItems);
            }
        }

        static private void CreateCatalogTypes(SqlServerProvider provider, IEnumerable<CatalogType> catalogTypes)
        {
            var dataSet = provider.GetDatasetSchema();
            var dataTable = dataSet.Tables["CatalogTypes"];
            foreach (var item in catalogTypes)
            {
                dataTable.Rows.Add(item.Id, item.Type);
            }
            provider.CreateCatalogTypes(dataSet);
        }

        static private void CreateCatalogBrands(SqlServerProvider provider, IEnumerable<CatalogBrand> catalogBrands)
        {
            var dataSet = provider.GetDatasetSchema();
            var dataTable = dataSet.Tables["CatalogBrands"];
            foreach (var item in catalogBrands)
            {
                dataTable.Rows.Add(item.Id, item.Brand);
            }
            provider.CreateCatalogBrands(dataSet);
        }

        static private DataTable CreateCatalogItems(SqlServerProvider provider, IEnumerable<CatalogItem> catalogItems)
        {
            var dataSet = provider.GetDatasetSchema();
            var dataTable = dataSet.Tables["CatalogItems"];
            foreach (var item in catalogItems.OrderBy(r => r.Id))
            {
                dataTable.Rows.Add(0, item.Name, item.Description, item.Price, item.CatalogTypeId, item.CatalogBrandId, $"{item.Id}.jpg");
            }
            provider.CreateCatalogItems(dataSet);
            return dataTable;
        }

        static private async Task CreateCatalogImages(SqlServerProvider provider, DataTable catalogItems)
        {
            foreach (DataRow row in catalogItems.Rows)
            {
                int id = (int)row["Id"];
                string name = row["PictureFileName"] as String;
                var image = await LoadImageAsync(id, name);
                provider.InsertImage(id, ".jpg", image);
            }
        }

        static private async Task<byte[]> LoadImageAsync(int id, string name)
        {
            string url = $"ms-appx:///Assets/Catalog/{name}";
            var storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(url, UriKind.Absolute));
            using (var randomStream = await storageFile.OpenReadAsync())
            {
                using (var stream = new BinaryReader(randomStream.AsStreamForRead()))
                {
                    return stream.ReadBytes((int)randomStream.Size);
                }
            }
        }
    }
}
