using System;

namespace eShop.UWP.Data
{
    public class CatalogItem
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }

        public string PictureFileName { get; set; }
        public string PictureUri { get; set; }

        public int CatalogTypeId { get; set; }
        public int CatalogBrandId { get; set; }

        public bool IsDisabled { get; set; }

        public DateTime LatUpdate { get; set; }
    }
}
