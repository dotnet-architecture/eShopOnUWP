using System;

namespace eShop.UWP.Models
{
    partial class CatalogItemModel
    {
        private bool _isDiscountEnabled;
        public bool IsDiscountEnabled
        {
            get { return _isDiscountEnabled; }
            set { Set(ref _isDiscountEnabled, value); UpdateFinalPrice(); }
        }

        private double _discountPercent;
        public double DiscountPercent
        {
            get { return _discountPercent; }
            set { if (Set(ref _discountPercent, value)) UpdateDiscountValue(); }
        }

        private double _discountValue;
        public double DiscountValue
        {
            get { return _discountValue; }
            set { if (Set(ref _discountValue, value)) UpdateDiscountPercent(); }
        }

        private double _finalPrice;
        public double FinalPrice
        {
            get { return _finalPrice; }
            set { Set(ref _finalPrice, value); }
        }

        private void UpdateDiscountPercent()
        {
            if (Price > 0.0)
            {
                DiscountPercent = Math.Round(100 * (DiscountValue / Price), 2);
            }
            else
            {
                DiscountPercent = 0.0;
            }
            UpdateFinalPrice();
        }

        private void UpdateDiscountValue()
        {
            DiscountValue = Math.Round(Price * (DiscountPercent / 100.0), 2);
            UpdateFinalPrice();
        }

        private void UpdateFinalPrice()
        {
            if (IsDiscountEnabled)
            {
                FinalPrice = Price - DiscountValue;
            }
            else
            {
                FinalPrice = Price;
            }
        }
    }
}
