using System;

namespace eShop.Domain.Models
{
    public class CatalogItem
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }

        public string PictureFileName { get; set; }
        public string PictureUri { get; set; }
        public byte[] Picture { get; set; }

        public int CatalogTypeId { get; set; }
        public CatalogType CatalogType { get; set; }

        public int CatalogBrandId { get; set; }
        public CatalogBrand CatalogBrand { get; set; }

        public bool IsActive { get; set; }
    }
}
