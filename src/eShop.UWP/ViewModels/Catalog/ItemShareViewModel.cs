using System;
using System.Threading.Tasks;

using eShop.UWP.Models;

namespace eShop.UWP.ViewModels
{
    public class ItemShareViewModel : CommonViewModel
    {
        public override bool AlwaysShowHeader => false;

        public CatalogItemModel Item { get; set; }

        public void Load(ItemShareState state)
        {
            Item = state.Item;
        }
    }
}
