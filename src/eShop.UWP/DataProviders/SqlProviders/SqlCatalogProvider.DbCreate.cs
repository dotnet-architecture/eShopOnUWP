using System;
using System.IO;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

using Windows.Storage;

using eShop.UWP;
using eShop.SqlProvider;
using eShop.UWP.Data;

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
                await CreateCatalogItems(provider, db.CatalogItems);
            }
        }

        static private void CreateCatalogTypes(SqlServerProvider provider, IEnumerable<CatalogType> catalogTypes)
        {
            foreach (var item in catalogTypes)
            {
                provider.InsertCatalogType(item.Id, item.Type);
            }
        }

        static private void CreateCatalogBrands(SqlServerProvider provider, IEnumerable<CatalogBrand> catalogBrands)
        {
            foreach (var item in catalogBrands)
            {
                provider.InsertCatalogBrand(item.Id, item.Brand);
            }
        }

        static private async Task CreateCatalogItems(SqlServerProvider provider, IEnumerable<CatalogItem> catalogItems)
        {
            foreach (var item in catalogItems.OrderBy(r => r.Id))
            {
                string pictureName = $"{item.Id}.jpg";
                provider.InsertCatalogItem(item.Id, item.Name, item.Description, pictureName, item.Price, item.CatalogTypeId, item.CatalogBrandId, item.IsDisabled);
                var picture = await LoadImageAsync(pictureName);
                provider.InsertCatalogImage(item.Id, picture);
            }
        }

        static private async Task<byte[]> LoadImageAsync(string name)
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
