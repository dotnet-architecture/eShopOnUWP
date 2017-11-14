using System;

using GalaSoft.MvvmLight;

using eShop.UWP.Data;

namespace eShop.UWP.Models
{
    public class CatalogTypeModel : ObservableObject
    {
        public CatalogTypeModel()
        {
            Id = 0;
            Name = "";
        }
        public CatalogTypeModel(CatalogType source)
        {
            Id = source.Id;
            Name = source.Type;
        }

        public int Id { get; set; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is CatalogTypeModel instance)
            {
                return instance.Id == Id;
            }
            return false;
        }
    }
}
