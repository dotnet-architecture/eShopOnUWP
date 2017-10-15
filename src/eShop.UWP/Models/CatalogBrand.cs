namespace eShop.Domain.Models
{
    public class CatalogBrand
    {
        public int Id { get; set; }
        public string Brand { get; set; }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is CatalogBrand instance)
            {
                return instance.Id == Id;
            }
            return false;
        }
    }
}
