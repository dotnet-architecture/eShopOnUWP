using System;
using System.Collections.ObjectModel;

using System.Windows;

using eShop.Providers;
using eShop.Models;

namespace eShop.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            DataContext = this;
        }

        #region Items
        public ObservableCollection<CatalogItemModel> Items
        {
            get { return (ObservableCollection<CatalogItemModel>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(ObservableCollection<CatalogItemModel>), typeof(MainWindow), new PropertyMetadata(null));
        #endregion

        #region Types
        public ObservableCollection<CatalogTypeModel> Types
        {
            get { return (ObservableCollection<CatalogTypeModel>)GetValue(TypesProperty); }
            set { SetValue(TypesProperty, value); }
        }

        public static readonly DependencyProperty TypesProperty = DependencyProperty.Register("Types", typeof(ObservableCollection<CatalogTypeModel>), typeof(MainWindow), new PropertyMetadata(null));
        #endregion

        #region Brands
        public ObservableCollection<CatalogBrandModel> Brands
        {
            get { return (ObservableCollection<CatalogBrandModel>)GetValue(BrandsProperty); }
            set { SetValue(BrandsProperty, value); }
        }

        public static readonly DependencyProperty BrandsProperty = DependencyProperty.Register("Brands", typeof(ObservableCollection<CatalogBrandModel>), typeof(MainWindow), new PropertyMetadata(null));
        #endregion

        #region Query
        public string Query
        {
            get { return (string)GetValue(QueryProperty); }
            set { SetValue(QueryProperty, value); }
        }

        public static readonly DependencyProperty QueryProperty = DependencyProperty.Register("Query", typeof(string), typeof(MainWindow), new PropertyMetadata(null));
        #endregion

        #region SelectedTypeId/SelectedBrandId
        public int SelectedTypeId
        {
            get { return (int)GetValue(SelectedTypeIdProperty); }
            set { SetValue(SelectedTypeIdProperty, value); }
        }

        public static readonly DependencyProperty SelectedTypeIdProperty = DependencyProperty.Register("SelectedTypeId", typeof(int), typeof(MainWindow), new PropertyMetadata(-1, OnFilterChanged));

        public int SelectedBrandId
        {
            get { return (int)GetValue(SelectedBrandProperty); }
            set { SetValue(SelectedBrandProperty, value); }
        }

        public static readonly DependencyProperty SelectedBrandProperty = DependencyProperty.Register("SelectedBrandId", typeof(int), typeof(MainWindow), new PropertyMetadata(-1, OnFilterChanged));

        private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MainWindow;
            control.UpdateItemList();
        }
        #endregion

        protected override void OnInitialized(EventArgs e)
        {
            var provider = new CatalogProvider();

            var types = provider.GetCatalogTypes();
            types.Insert(0, new CatalogTypeModel());
            Types = new ObservableCollection<CatalogTypeModel>(types);

            var brands = provider.GetCatalogBrands();
            brands.Insert(0, new CatalogBrandModel());
            Brands = new ObservableCollection<CatalogBrandModel>(brands);

            UpdateItemList();

            base.OnInitialized(e);
        }

        private void UpdateItemList()
        {
            var provider = new CatalogProvider();

            var items = provider.GetItems(SelectedTypeId, SelectedBrandId, Query);
            Items = new ObservableCollection<CatalogItemModel>(items);
        }

        private void OnSearchClick(object sender, RoutedEventArgs e)
        {
            UpdateItemList();
        }
    }
}
