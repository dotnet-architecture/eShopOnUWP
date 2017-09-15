using System.Collections.Generic;
using System.Linq;
using eShop.Domain.Models;
using eShop.Providers.Contracts;

namespace eShop.Providers
{
    public class CatalogProvider : ICatalogProvider
    {
        private const string ImageBaseUri = "ms-appx:///Assets/Images/Catalog/";

        private static readonly List<CatalogBrand> Brands = GetPreconfiguredCatalogBrands();
        private static readonly List<CatalogType> Types = GetPreconfiguredCatalogTypes();
        private static readonly List<CatalogItem> Items = GenerateFullCatalogList();

        public IList<CatalogBrand> GetCatalogBrands()
        {
            return Brands;
        }

        public IList<CatalogType> GetCatalogTypes()
        {
            return Types;
        }

        public IList<CatalogItem> GetItems(CatalogType selectedCatalogType, CatalogBrand selectedCatalogBrand, string query)
        {
            var queryIgnoreUpper = query?.ToUpperInvariant() ?? string.Empty;

            var itemFiltered = !string.IsNullOrEmpty(queryIgnoreUpper) ? Items.Where(item => item.Name.ToUpperInvariant().Contains(queryIgnoreUpper)).ToList() : Items;

            if (selectedCatalogType != null && selectedCatalogType.Id != 0)
            {
                itemFiltered = itemFiltered.Where(item => item.CatalogType.Id == selectedCatalogType.Id).ToList();
            }

            if (selectedCatalogBrand != null && selectedCatalogBrand.Id != 0)
            {
                itemFiltered = itemFiltered.Where(item => item.CatalogBrand.Id == selectedCatalogBrand.Id).ToList();
            }

            return itemFiltered;
        }

        public IList<CatalogItem> GetItemsByVoiceCommand(string query)
        {
            var queryIgnoreUpper = query?.ToUpperInvariant() ?? string.Empty;

            var filterType = Types.FirstOrDefault(item => item.Type.ToUpperInvariant().Contains(queryIgnoreUpper));
            if (filterType != null)
            {
                return Items.Where(item => item.CatalogType.Id == filterType.Id).ToList();
            }

            var filterBrandType = Brands.FirstOrDefault(item => item.Brand.ToUpperInvariant().Contains(queryIgnoreUpper));
            if (filterBrandType != null)
            {
                return Items.Where(item => item.CatalogBrand.Id == filterBrandType.Id).ToList();
            }

            return Items.ToList();
        }

        public CatalogItem GetItemById(int id)
        {
            return Items.FirstOrDefault(item => item.Id == id);
        }

        public IList<CatalogItem> RelatedItemsByType(int catalogTypeId)
        {
            return catalogTypeId == 0 ? Items : Items.Where(item => item.CatalogTypeId == catalogTypeId).ToList();
        }

        public void SaveItem(CatalogItem catalogItem)
        {
            if (catalogItem.Id != 0)
            {
                var oldItem = Items.FirstOrDefault(item => item.Id == catalogItem.Id);
                var index = 0;
                if (oldItem != null)
                {
                    index = Items.IndexOf(oldItem);
                    Items.Remove(oldItem);
                }

                Items.Insert(index, catalogItem);
            }
            else
            {
                var lastItem = Items.OrderBy(item => item.Id).LastOrDefault();
                catalogItem.Id = lastItem != null ? lastItem.Id + 1 : 1;
                Items.Add(catalogItem);
            }
        }

        public void DeleteItem(CatalogItem catalogItem)
        {
            if (catalogItem.Id == 0) return;

            Items.RemoveAll(item => item.Id == catalogItem.Id);
        }

        private static List<CatalogItem> GenerateFullCatalogList()
        {
            var list = new List<CatalogItem>();
            Enumerable.Range(0, 2).ToList().ForEach(arg => list.AddRange(GenerateMockList()));

            var id = 1;
            list.ForEach(item => item.Id = id++);

            return list;
        }

        private static IEnumerable<CatalogItem> GenerateMockList()
        {
            return GetPreconfiguredItems().Select(item =>
            {
                item.CatalogType = Types.FirstOrDefault(type => type.Id == item.CatalogTypeId);
                item.CatalogBrand = Brands.FirstOrDefault(brand => brand.Id == item.CatalogBrandId);
                return item;
            });
        }

        private static List<CatalogBrand> GetPreconfiguredCatalogBrands()
        {
            return new List<CatalogBrand>
            {
                new CatalogBrand
                {
                    Id = 1,
                    Brand = "Azure"
                },
                new CatalogBrand
                {
                    Id = 2,
                    Brand = ".NET"
                },
                new CatalogBrand
                {
                    Id = 5,
                    Brand = "Other"
                }
            };
        }

        private static List<CatalogType> GetPreconfiguredCatalogTypes()
        {
            return new List<CatalogType>
            {
                new CatalogType
                {
                    Id = 1,
                    Type = "Mug"
                },
                new CatalogType
                {
                    Id = 2,
                    Type = "Shirt"
                },
                new CatalogType
                {
                    Id = 3,
                    Type = "Sheet"
                },
                new CatalogType
                {
                    Id = 4,
                    Type = "Cap"
                }
            };
        }

        private static List<CatalogItem> GetPreconfiguredItems()
        {
            return new List<CatalogItem>
            {
                new CatalogItem
                {
                    Id = 1,
                    CatalogTypeId = 2,
                    IsActive = true,
                    CatalogBrandId = 2,
                    Description = ".NET Blue Hoodie",
                    Name = ".NET Blue Hoodie",
                    Price = 19.5,
                    PictureUri = $"{ImageBaseUri}1.png"
                },
                new CatalogItem
                {
                    Id = 13,
                    CatalogTypeId = 4,
                    IsActive = false,
                    CatalogBrandId = 2,
                    Description = ".NET Violet Cap",
                    Name = ".NET Violet Cap",
                    Price = 12,
                    PictureUri = $"{ImageBaseUri}13.png"
                },
                new CatalogItem
                {
                    Id = 4,
                    CatalogTypeId = 2,
                    IsActive = true,
                    CatalogBrandId = 5,
                    Description = "Cup<T> White T-Shirt",
                    Name = "Cup<T> White T-Shirt",
                    Price = 12,
                    PictureUri = $"{ImageBaseUri}4.png"
                },
                new CatalogItem
                {
                    Id = 8,
                    CatalogTypeId = 3,
                    IsActive = true,
                    CatalogBrandId = 1,
                    Description = ".NET Violet Sheet",
                    Name = ".NET Violet Sheet",
                    Price = 8.5,
                    PictureUri = $"{ImageBaseUri}8.png"
                },
                new CatalogItem
                {
                    Id = 6,
                    CatalogTypeId = 2,
                    IsActive = true,
                    CatalogBrandId = 5,
                    Description = "Roslyn Red Shirt",
                    Name = "ROSLYN RED SHEET",
                    Price = 12,
                    PictureUri = $"{ImageBaseUri}6.png"
                },
                new CatalogItem
                {
                    Id = 9,
                    CatalogTypeId = 3,
                    IsActive = true,
                    CatalogBrandId = 5,
                    Description = "Roslyn Red Sheet",
                    Name = "ROSLYN RED SHEET",
                    Price = 8.5,
                    PictureUri = $"{ImageBaseUri}9.png"
                },
                new CatalogItem
                {
                    Id = 10,
                    CatalogTypeId = 3,
                    CatalogBrandId = 5,
                    IsActive = true,
                    Description = "Cup<T> White Sheet",
                    Name = "CUP<T> WHITE SHEET",
                    Price = 8.5,
                    PictureUri = $"{ImageBaseUri}10.png"
                },
                new CatalogItem
                {
                    Id = 3,
                    CatalogTypeId = 2,
                    IsActive = true,
                    CatalogBrandId = 1,
                    Description = "Azure Violet Hoodie",
                    Name = "Azure Violet Hoodie",
                    Price = 19.5,
                    PictureUri = $"{ImageBaseUri}3.png"
                },
                new CatalogItem
                {
                    Id = 11,
                    CatalogTypeId = 1,
                    CatalogBrandId = 2,
                    IsActive = true,
                    Description = ".NET Foundation Mug",
                    Name = ".NET FOUNDATION MUG",
                    Price = 12,
                    PictureUri = $"{ImageBaseUri}11.png"
                },
                new CatalogItem
                {
                    Id = 5,
                    CatalogTypeId = 2,
                    IsActive = true,
                    CatalogBrandId = 2,
                    Description = ".NET Foundation T-shirt",
                    Name = ".NET FOUNDATION T-SHIRT",
                    Price = 12,
                    PictureUri = $"{ImageBaseUri}5.png"
                },
                new CatalogItem
                {
                    Id = 12,
                    CatalogTypeId = 1,
                    IsActive = false,
                    CatalogBrandId = 5,
                    Description = "Cup<T> Mug",
                    Name = "CUP<T> MUG",
                    Price = 12,
                    PictureUri = $"{ImageBaseUri}12.png"
                },
                new CatalogItem
                {
                    Id = 2,
                    CatalogTypeId = 2,
                    IsActive = true,
                    CatalogBrandId = 5,
                    Description = "Bot Black Hoodie",
                    Name = "Bot Black Hoodie",
                    Price = 19.5,
                    PictureUri = $"{ImageBaseUri}2.png"
                },
                new CatalogItem
                {
                    Id = 14,
                    CatalogTypeId = 4,
                    IsActive = false,
                    CatalogBrandId = 5,
                    Description = "Roslyn Red Cap",
                    Name = "ROSLYN RED CAP",
                    Price = 12,
                    PictureUri = $"{ImageBaseUri}14.png"
                },
                new CatalogItem
                {
                    Id = 7,
                    CatalogTypeId = 2,
                    IsActive = true,
                    CatalogBrandId = 2,
                    Description = "Prism White T-Shirt",
                    Name = "PRISM WHITE T-SHIRT",
                    Price = 12,
                    PictureUri = $"{ImageBaseUri}7.png"
                },
            };
        }
    }
}
