using System;

using eShop.UWP.Models;

namespace eShop.UWP.ViewModels
{
    public class ItemShareState : ViewState
    {
        public ItemShareState(CatalogItemModel item)
        {
            Item = item;
        }

        public CatalogItemModel Item { get; set; }
    }
}
