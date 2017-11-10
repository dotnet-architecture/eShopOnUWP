using System;
using System.Data;

namespace eShop.SqlProvider
{
    using Properties;
    using CatalogDSTableAdapters;

    public partial class SqlServerProvider
    {
        public SqlServerProvider(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString
        {
            get { return Settings.Default.ConnectionString; }
            set { Settings.Default.ConnectionString = value; }
        }

        // GET

        public DataTable GetCatalogTypes()
        {
            CatalogTypesTableAdapter dataAdapter = new CatalogTypesTableAdapter();
            return dataAdapter.GetData();
        }

        public DataTable GetCatalogBrands()
        {
            CatalogBrandsTableAdapter dataAdapter = new CatalogBrandsTableAdapter();
            return dataAdapter.GetData();
        }

        public DataTable GetCatalogItem(int id)
        {
            CatalogItemTableAdapter dataAdapter = new CatalogItemTableAdapter();
            return dataAdapter.GetData(id);
        }

        public DataTable GetCatalogItems()
        {
            CatalogItemsTableAdapter dataAdapter = new CatalogItemsTableAdapter();
            return dataAdapter.GetData();
        }

        public DataTable GetCatalogItemsFilter(int typeId = -1, int brandId = -1, string query = null)
        {
            CatalogItemsFilterTableAdapter dataAdapter = new CatalogItemsFilterTableAdapter();
            return dataAdapter.GetData(typeId, brandId, query);
        }

        public DataTable GetCatalogImage(int id)
        {
            CatalogPictureTableAdapter dataAdapter = new CatalogPictureTableAdapter();
            return dataAdapter.GetData(id);
        }

        // INSERT

        public int InsertCatalogType(int id, string name)
        {
            CatalogTypesTableAdapter dataAdapter = new CatalogTypesTableAdapter();
            return dataAdapter.Insert(id, name);
        }

        public int InsertCatalogBrand(int id, string name)
        {
            CatalogBrandsTableAdapter dataAdapter = new CatalogBrandsTableAdapter();
            return dataAdapter.Insert(id, name);
        }

        public int InsertCatalogItem(int id, string name, string description, string pictureName, double price, int typeId, int brandId, bool isDisabled)
        {
            CatalogItemsTableAdapter dataAdapter = new CatalogItemsTableAdapter();
            return dataAdapter.Insert(id, name, description, pictureName, price, typeId, brandId, isDisabled, DateTime.UtcNow);
        }

        public int InsertCatalogImage(int id, byte[] imageBytes)
        {
            // TODO: Check if exists
            CatalogPicturesTableAdapter dataAdapter = new CatalogPicturesTableAdapter();
            return dataAdapter.Insert(id, imageBytes);
        }

        // UPDATE

        public int UpdateCatalogType(int id, string type)
        {
            CatalogTypesTableAdapter dataAdapter = new CatalogTypesTableAdapter();
            return dataAdapter.Update(type, id);
        }
        public int UpdateCatalogType(DataRow dataRow)
        {
            CatalogTypesTableAdapter dataAdapter = new CatalogTypesTableAdapter();
            return dataAdapter.Update(dataRow);
        }

        public int UpdateCatalogBrand(int id, string brand)
        {
            CatalogBrandsTableAdapter dataAdapter = new CatalogBrandsTableAdapter();
            return dataAdapter.Update(brand, id);
        }
        public int UpdateCatalogBrand(DataRow dataRow)
        {
            CatalogBrandsTableAdapter dataAdapter = new CatalogBrandsTableAdapter();
            return dataAdapter.Update(dataRow);
        }

        public int UpdateCatalogItem(int id, string name, string description, string pictureName, double price, int typeId, int brandId, bool isDisabled)
        {
            CatalogItemsTableAdapter dataAdapter = new CatalogItemsTableAdapter();
            return dataAdapter.Update(name, description, pictureName, price, typeId, brandId, isDisabled, DateTime.UtcNow, id);
        }
        public int UpdateCatalogItem(DataRow dataRow)
        {
            CatalogItemsTableAdapter dataAdapter = new CatalogItemsTableAdapter();
            return dataAdapter.Update(dataRow);
        }

        public int UpdateCatalogImage(int id, byte[] imageBytes)
        {
            CatalogPicturesTableAdapter dataAdapter = new CatalogPicturesTableAdapter();
            return dataAdapter.Update(imageBytes, id);
        }
        public int UpdateCatalogImage(DataRow dataRow)
        {
            CatalogPicturesTableAdapter dataAdapter = new CatalogPicturesTableAdapter();
            return dataAdapter.Update(dataRow);
        }

        // DELETE

        public int DeleteCatalogType(int id)
        {
            CatalogTypesTableAdapter dataAdapter = new CatalogTypesTableAdapter();
            return dataAdapter.Delete(id);
        }

        public int DeleteCatalogBrand(int id)
        {
            CatalogBrandsTableAdapter dataAdapter = new CatalogBrandsTableAdapter();
            return dataAdapter.Delete(id);
        }

        public int DeleteCatalogItem(int id)
        {
            CatalogItemsTableAdapter dataAdapter = new CatalogItemsTableAdapter();
            return dataAdapter.Delete(id);
        }

        public int DeleteCatalogImage(int id)
        {
            CatalogPicturesTableAdapter dataAdapter = new CatalogPicturesTableAdapter();
            return dataAdapter.Delete(id);
        }
    }
}
