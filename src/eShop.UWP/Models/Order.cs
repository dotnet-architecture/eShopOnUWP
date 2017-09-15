using System;

namespace eShop.Domain.Models
{
    public class Order
    {
        public DateTime OrderDate { get; set; }
        public int CatalogTypeId { get; set; }
        public double OrderTotal { get; set; }
    }
}
