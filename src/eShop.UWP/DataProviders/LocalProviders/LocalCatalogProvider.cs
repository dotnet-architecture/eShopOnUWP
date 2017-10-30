using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using eShop.Domain.Models;
using eShop.Providers.Contracts;
using eShop.UWP;

namespace eShop.Providers
{
    public class LocalCatalogProvider : ICatalogProvider
    {
        public Task<Result> IsAvailableAsync()
        {
            return Task.FromResult(Result.Ok());
        }

        public async Task<IList<CatalogType>> GetCatalogTypesAsync()
        {
            await Task.FromResult(true);
            using (var db = new LocalCatalogDb())
            {
                return db.CatalogTypes;
            }
        }

        public async Task<IList<CatalogBrand>> GetCatalogBrandsAsync()
        {
            await Task.FromResult(true);
            using (var db = new LocalCatalogDb())
            {
                return db.CatalogBrands;
            }
        }

        public async Task<CatalogItem> GetItemByIdAsync(int id)
        {
            await Task.FromResult(true);
            using (var db = new LocalCatalogDb())
            {
                var item = db.CatalogItems.FirstOrDefault(r => r.Id == id);
                Populate(db, item);
                return item;
            }
        }

        public async Task<IList<CatalogItem>> GetItemsAsync(CatalogType catalogType, CatalogBrand catalogBrand, string query)
        {
            await Task.FromResult(true);
            using (var db = new LocalCatalogDb())
            {
                IEnumerable<CatalogItem> items = db.CatalogItems;

                if (!String.IsNullOrEmpty(query))
                {
                    items = items.Where(r => $"{r.Name}".ToUpper().Contains(query.ToUpper()));
                }

                if (catalogType != null && catalogType.Id > 0)
                {
                    items = items.Where(r => r.CatalogTypeId == catalogType.Id);
                }

                if (catalogBrand != null && catalogBrand.Id > 0)
                {
                    items = items.Where(r => r.CatalogBrandId == catalogBrand.Id);
                }

                return Populate(db, items.ToArray().OrderBy(r => r.Name)).ToList();
            }
        }

        public async Task<IList<CatalogItem>> GetItemsByVoiceCommandAsync(string query)
        {
            await Task.FromResult(true);

            using (var db = new LocalCatalogDb())
            {
                IEnumerable<CatalogItem> items = db.CatalogItems;

                var queryIgnoreUpper = query?.ToUpperInvariant() ?? string.Empty;

                var filterType = db.CatalogTypes.FirstOrDefault(item => item.Type.ToUpperInvariant().Contains(queryIgnoreUpper));
                if (filterType != null)
                {
                    items = items.Where(item => item.CatalogType.Id == filterType.Id);
                }

                var filterBrand = db.CatalogBrands.FirstOrDefault(item => item.Brand.ToUpperInvariant().Contains(queryIgnoreUpper));
                if (filterBrand != null)
                {
                    items = items.Where(item => item.CatalogBrand.Id == filterBrand.Id);
                }

                return Populate(db, items.ToArray().OrderBy(r => r.Name)).ToList();
            }
        }

        public async Task<IList<CatalogItem>> RelatedItemsByTypeAsync(int catalogTypeId)
        {
            await Task.FromResult(true);
            using (var db = new LocalCatalogDb())
            {
                var items = catalogTypeId == 0 ? db.CatalogItems : db.CatalogItems.Where(r => r.CatalogTypeId == catalogTypeId);
                return Populate(db, items).ToList();
            }
        }

        public async Task SaveItemAsync(CatalogItem item)
        {
            await Task.FromResult(true);
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
                Populate(db, item);
                db.CatalogItems.Add(item);
                db.SaveChanges();
            }
        }

        public async Task DeleteItemAsync(CatalogItem item)
        {
            await Task.FromResult(true);
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

        private IEnumerable<CatalogItem> Populate(LocalCatalogDb db, IEnumerable<CatalogItem> items)
        {
            foreach (var item in items)
            {
                Populate(db, item);
            }
            return items;
        }

        private CatalogItem Populate(LocalCatalogDb db, CatalogItem item)
        {
            if (item != null)
            {
                item.CatalogType = db.CatalogTypes.FirstOrDefault(r => r.Id == item.CatalogTypeId);
                item.CatalogBrand = db.CatalogBrands.FirstOrDefault(r => r.Id == item.CatalogBrandId);
            }
            return item;
        }
    }
}
