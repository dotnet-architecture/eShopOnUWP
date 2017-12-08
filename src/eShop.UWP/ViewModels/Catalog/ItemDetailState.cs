using System;

using eShop.UWP.Models;

namespace eShop.UWP.ViewModels
{
    public class ItemDetailState : ViewState
    {
        public ItemDetailState(int id = 0)
        {
            Item = new CatalogItemModel(id);
        }
        public ItemDetailState(CatalogItemModel item) : this()
        {
            Item = item;
        }

        public CatalogItemModel Item { get; set; }
    }
}
