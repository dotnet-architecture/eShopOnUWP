using System;

using System.Windows;

using eShop.Data;
using eShop.Providers;

namespace eShop.Models
{
    public class CatalogItemModel : DependencyObject
    {
        public CatalogItemModel(int id = 0) : this(new CatalogItem { Id = id })
        {
        }
        public CatalogItemModel(CatalogItem source)
        {
            Source = source;
            CopyValues(Source);
        }

        public CatalogItem Source { get; private set; }

        public int Id { get => Source.Id; }

        #region Name
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(CatalogItemModel), new PropertyMetadata(null));
        #endregion

        #region Description
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(CatalogItemModel), new PropertyMetadata(null));
        #endregion

        #region Price
        public double Price
        {
            get { return (double)GetValue(PriceProperty); }
            set { SetValue(PriceProperty, value); }
        }

        public static readonly DependencyProperty PriceProperty = DependencyProperty.Register("Price", typeof(double), typeof(CatalogItemModel), new PropertyMetadata(null));
        #endregion

        public string PriceDesc => $"${Price.ToString("0.00")}";

        public string PriceString
        {
            get { return Price.ToString("0.00"); }
            set { Price = ParseDecimal(value); }
        }

        private double ParseDecimal(string value)
        {
            double d = 0;
            Double.TryParse(value, out d);
            return d;
        }

        public string PictureFileName { get; set; }
        public byte[] Picture { get; set; }
        public string PictureContentType { get; set; }

        #region PictureUri
        public string PictureUri
        {
            get { return (string)GetValue(PictureUriProperty); }
            set { SetValue(PictureUriProperty, value); }
        }

        public static readonly DependencyProperty PictureUriProperty = DependencyProperty.Register("PictureUri", typeof(string), typeof(CatalogItemModel), new PropertyMetadata(null));
        #endregion

        public CatalogTypeModel CatalogType { get; set; }
        public CatalogBrandModel CatalogBrand { get; set; }

        #region IsDisabled
        public bool IsDisabled
        {
            get { return (bool)GetValue(IsDisabledProperty); }
            set { SetValue(IsDisabledProperty, value); }
        }

        public static readonly DependencyProperty IsDisabledProperty = DependencyProperty.Register("IsDisabled", typeof(bool), typeof(CatalogItemModel), new PropertyMetadata(false));
        #endregion

        // Management Properties

        public bool IsSelected { get; set; }

        #region IsNew
        public bool IsNew
        {
            get { return (bool)GetValue(IsNewProperty); }
            set { SetValue(IsNewProperty, value); }
        }

        public static readonly DependencyProperty IsNewProperty = DependencyProperty.Register("IsNew", typeof(bool), typeof(CatalogItemModel), new PropertyMetadata(null));
        #endregion

        #region IsDeleted
        public bool IsDeleted
        {
            get { return (bool)GetValue(IsDeletedProperty); }
            set { SetValue(IsDeletedProperty, value); }
        }

        public static readonly DependencyProperty IsDeletedProperty = DependencyProperty.Register("IsDeleted", typeof(bool), typeof(CatalogItemModel), new PropertyMetadata(null));
        #endregion

        public bool HasChanges
        {
            get
            {
                return
                    Source.Name != Name ||
                    Source.Description != Description ||
                    Source.Price != Price ||
                    Source.PictureUri != PictureUri ||
                    Source.CatalogTypeId != CatalogType.Id ||
                    Source.CatalogBrandId != CatalogBrand.Id ||
                    Source.IsDisabled != IsDisabled;
            }
        }

        public void Undo()
        {
            CopyValues(Source);

            IsDeleted = false;
            //RaisePropertyChanged(nameof(HasChanges));
        }

        private void CopyValues(CatalogItem source)
        {
            Name = source.Name;
            Description = source.Description;
            Price = source.Price;
            PictureUri = source.PictureUri;
            CatalogType = CatalogProvider.GetCatalogType(source.CatalogTypeId) ?? new CatalogTypeModel();
            CatalogBrand = CatalogProvider.GetCatalogBrand(source.CatalogBrandId) ?? new CatalogBrandModel();
            IsDisabled = source.IsDisabled;
        }

        public void Commit()
        {
            Source.Name = Name;
            Source.Description = Description;
            Source.Price = Price;
            Source.PictureUri = PictureUri;
            Source.CatalogTypeId = CatalogType.Id;
            Source.CatalogBrandId = CatalogBrand.Id;
            Source.IsDisabled = IsDisabled;

            //RaisePropertyChanged(nameof(HasChanges));
        }
    }
}
