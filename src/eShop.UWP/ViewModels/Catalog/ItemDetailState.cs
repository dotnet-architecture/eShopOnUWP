using System;

using eShop.UWP.Models;

namespace eShop.UWP.ViewModels
{
    public class ItemDetailState : ViewState
    {
        public ItemDetailState()
        {
            Item = null;
        }
        public ItemDetailState(CatalogItemModel item) : this()
        {
            Item = item;
        }

        public CatalogItemModel Item { get; set; }
    }
}
