using System;
using System.Linq;
using System.Collections.Generic;

using eShop.Domain.Models;
using eShop.Providers.Contracts;

namespace eShop.Providers
{
    public class LocalCatalogProvider : ICatalogProvider
    {
        public IList<CatalogType> GetCatalogTypes()
        {
            using (var db = new LocalCatalogDb())
            {
                return db.CatalogTypes;
            }
        }

        public IList<CatalogBrand> GetCatalogBrands()
        {
            using (var db = new LocalCatalogDb())
            {
                return db.CatalogBrands;
            }
        }

        public CatalogItem GetItemById(int id)
        {
            using (var db = new LocalCatalogDb())
            {
                var item = db.CatalogItems.FirstOrDefault(r => r.Id == id);
                Populate(db, item);
                return item;
            }
        }

        public IList<CatalogItem> GetItems(CatalogType selectedCatalogType, CatalogBrand selectedCatalogBrand, string query)
        {
            using (var db = new LocalCatalogDb())
            {
                IEnumerable<CatalogItem> items = db.CatalogItems;

                if (!String.IsNullOrEmpty(query))
                {
                    items = items.Where(r => $"{r.Name}".ToUpper().Contains(query.ToUpper()));
                }

                if (selectedCatalogType != null && selectedCatalogType.Id != 0)
                {
                    items = items.Where(r => r.CatalogTypeId == selectedCatalogType.Id);
                }

                if (selectedCatalogBrand != null && selectedCatalogBrand.Id != 0)
                {
                    items = items.Where(r => r.CatalogBrandId == selectedCatalogBrand.Id);
                }

                return Populate(db, items.ToArray().OrderBy(r => r.Name)).ToList();
            }
        }

        public IList<CatalogItem> GetItemsByVoiceCommand(string query)
        {
            // TODO:
            return new CatalogItem[] { }.ToList();
        }

        public IList<CatalogItem> RelatedItemsByType(int catalogTypeId)
        {
            using (var db = new LocalCatalogDb())
            {
                var items = catalogTypeId == 0 ? db.CatalogItems : db.CatalogItems.Where(r => r.CatalogTypeId == catalogTypeId);
                return Populate(db, items).ToList();
            }
        }

        public void SaveItem(CatalogItem item)
        {
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

        public void DeleteItem(CatalogItem item)
        {
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
