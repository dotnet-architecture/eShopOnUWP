using System;

using System.Windows;

using eShop.Data;

namespace eShop.Models
{
    public class CatalogBrandModel : DependencyObject
    {
        public CatalogBrandModel()
        {
            Id = -1;
            Name = "View All";
        }
        public CatalogBrandModel(CatalogBrand source)
        {
            Id = source.Id;
            Name = source.Brand;
        }

        public int Id { get; set; }

        #region Name
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(CatalogBrandModel), new PropertyMetadata(null));
        #endregion
    }
}
