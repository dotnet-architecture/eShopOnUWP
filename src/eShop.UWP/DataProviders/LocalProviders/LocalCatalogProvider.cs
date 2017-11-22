using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using eShop.UWP;
using eShop.Data;
using eShop.UWP.Models;

namespace eShop.Providers
{
    public class LocalCatalogProvider : ICatalogProvider
    {
        public string Name => "Local";

        public Task<Result> IsAvailableAsync()
        {
            return Task.FromResult(Result.Ok());
        }

        public async Task<IList<CatalogTypeModel>> GetCatalogTypesAsync()
        {
            await Task.CompletedTask;
            using (var db = new LocalCatalogDb())
            {
                return db.CatalogTypes.Select(r => new CatalogTypeModel(r)).ToList();
            }
        }

        public async Task<IList<CatalogBrandModel>> GetCatalogBrandsAsync()
        {
            await Task.CompletedTask;
            using (var db = new LocalCatalogDb())
            {
                return db.CatalogBrands.Select(r => new CatalogBrandModel(r)).ToList();
            }
        }

        public async Task<CatalogItemModel> GetItemByIdAsync(int id)
        {
            await Task.CompletedTask;
            using (var db = new LocalCatalogDb())
            {
                var item = db.CatalogItems.Where(r => r.Id == id).Select(r => new CatalogItemModel(r)).FirstOrDefault();
                return item;
            }
        }

        public async Task<IList<CatalogItemModel>> GetItemsAsync(int typeId, int brandId, string query)
        {
            await Task.CompletedTask;
            using (var db = new LocalCatalogDb())
            {
                IEnumerable<CatalogItem> items = db.CatalogItems;

                if (!String.IsNullOrEmpty(query))
                {
                    items = items.Where(r => $"{r.Name}".ToUpper().Contains(query.ToUpper()));
                }

                if (typeId > -1)
                {
                    items = items.Where(r => r.CatalogTypeId == typeId);
                }

                if (brandId > -1)
                {
                    items = items.Where(r => r.CatalogBrandId == brandId);
                }

                return items.Select(r => new CatalogItemModel(r)).OrderBy(r => r.Name).ToList();
            }
        }

        public async Task<IList<CatalogItemModel>> GetItemsByVoiceCommandAsync(string query)
        {
            await Task.CompletedTask;

            using (var db = new LocalCatalogDb())
            {
                IEnumerable<CatalogItem> items = db.CatalogItems;

                var queryIgnoreUpper = query?.ToUpperInvariant() ?? string.Empty;

                var filterType = db.CatalogTypes.FirstOrDefault(item => item.Type.ToUpperInvariant().Contains(queryIgnoreUpper));
                if (filterType != null)
                {
                    items = items.Where(item => item.CatalogTypeId == filterType.Id);
                }

                var filterBrand = db.CatalogBrands.FirstOrDefault(item => item.Brand.ToUpperInvariant().Contains(queryIgnoreUpper));
                if (filterBrand != null)
                {
                    items = items.Where(item => item.CatalogBrandId == filterBrand.Id);
                }

                return items.Select(r => new CatalogItemModel(r)).OrderBy(r => r.Name).ToList();
            }
        }

        public async Task<IList<CatalogItemModel>> RelatedItemsByTypeAsync(int typeId)
        {
            await Task.CompletedTask;
            using (var db = new LocalCatalogDb())
            {
                var items = typeId <= 0 ? db.CatalogItems : db.CatalogItems.Where(r => r.CatalogTypeId == typeId);
                return items.Select(r => new CatalogItemModel(r)).OrderBy(r => r.Name).ToList();
            }
        }

        public async Task SaveItemAsync(CatalogItemModel model)
        {
            await Task.CompletedTask;
            var item = model.Source;
            using (var db = new LocalCatalogDb())
            {
                var oldItem = db.CatalogItems.FirstOrDefault(r => r.Id == item.Id);
                if (oldItem != null)
                {
                    db.CatalogItems.Remove(oldItem);
                }
                if (item.Id == 0)
                {
                    item.Id = 1;
                    if (db.CatalogItems.Count > 0)
                    {
                        item.Id = db.CatalogItems.Max(r => r.Id) + 1;
                    }
                }
                db.CatalogItems.Add(item);
                db.SaveChanges();
            }
        }

        public async Task DeleteItemAsync(CatalogItemModel item)
        {
            await Task.CompletedTask;
            using (var db = new LocalCatalogDb())
            {
                var oldItem = db.CatalogItems.FirstOrDefault(r => r.Id == item.Id);
                if (oldItem != null)
                {
                    db.CatalogItems.Remove(oldItem);
                }
                db.SaveChanges();
            }
        }
    }
}
