using System;

namespace eShop.Domain.Models
{
    public class CatalogType
    {
        public int Id { get; set; }
        public string Type { get; set; }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is CatalogType instance)
            {
                return instance.Id == Id;
            }
            return false;
        }
    }
}
