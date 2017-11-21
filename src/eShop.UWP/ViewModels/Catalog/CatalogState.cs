using System;

namespace eShop.UWP.ViewModels
{
    public class CatalogState : ViewState
    {
        public CatalogState()
        {
            FilterTypeId = -1;
            FilterBrandId = -1;
            IsGridChecked = true;
            IsListChecked = false;
            Query = null;
            SelectedItemId = 0;
        }
        public CatalogState(string query) : this()
        {
            Query = query;
        }

        public int FilterTypeId { get; set; }
        public int FilterBrandId { get; set; }

        public bool IsGridChecked { get; set; }
        public bool IsListChecked { get; set; }

        public string Query { get; set; }

        public int SelectedItemId { get; set; }
    }
}
