using System;

namespace eShop.UWP.ViewModels
{
    public class CatalogState : ViewState
    {
        public CatalogState()
        {
            IsGridChecked = true;
            IsListChecked = false;
            Query = null;
        }
        public CatalogState(string query) : this()
        {
            Query = query;
        }

        public bool IsGridChecked { get; set; }
        public bool IsListChecked { get; set; }

        public string Query { get; set; }
    }
}
