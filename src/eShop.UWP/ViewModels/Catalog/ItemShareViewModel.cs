using System;
using System.Threading.Tasks;

using eShop.UWP.Models;

namespace eShop.UWP.ViewModels
{
    public class ItemShareViewModel : CommonViewModel
    {
        public override bool AlwaysShowHeader => false;

        private CatalogItemModel _item;
        public CatalogItemModel Item
        {
            get { return _item ?? CatalogItemModel.Empty; }
            set { Set(ref _item, value); }
        }

        public bool HasPicture => !String.IsNullOrWhiteSpace(Item?.PictureUri);

        public void Load(ItemShareState state)
        {
            Item = state.Item;
        }
    }
}
