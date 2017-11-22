using System;

using System.Windows;

using eShop.Data;

namespace eShop.Models
{
    public class CatalogTypeModel : DependencyObject
    {
        public CatalogTypeModel()
        {
            Id = -1;
            Name = "View All";
        }
        public CatalogTypeModel(CatalogType source)
        {
            Id = source.Id;
            Name = source.Type;
        }

        public int Id { get; set; }

        #region Name
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(CatalogTypeModel), new PropertyMetadata(null));
        #endregion
    }
}
