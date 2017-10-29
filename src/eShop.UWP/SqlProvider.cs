using System;
using System.Data;
using System.Data.SqlClient;

namespace eShop.SqlProvider
{
    public class CatalogProvider
    {
        const string QUERY_TYPES = "SELECT * FROM CatalogTypes";
        const string QUERY_GBRANDS = "SELECT * FROM CatalogBrands";
        const string QUERY_ITEMS = "SELECT [Id], [Name], [Description], [Price], [CatalogTypeId], [CatalogBrandId] FROM CatalogItems";
        const string QUERY_ITEMSBYID = "SELECT [Id], [Name], [Description], [Price], [CatalogTypeId], [CatalogBrandId] FROM CatalogItems WHERE Id=@Id";
        const string CREATE_ITEMS = "INSERT INTO CatalogItems ([Name], [Description], [Price], [CatalogTypeId], [CatalogBrandId]) VALUES (@Name, @Description, @Price, @CatalogTypeId, @CatalogBrandId) SET @Id = SCOPE_IDENTITY()";
        const string UPDATE_ITEMS = "UPDATE CatalogItems SET [Name] = @Name, [Description] = @Description, [Price] = @Price, [CatalogTypeId] = @CatalogTypeId, [CatalogBrandId] = @CatalogBrandId WHERE [Id] = @Id";
        const string DELETE_ITEM = "DELETE FROM CatalogItems WHERE [Id] = @Id";

        public CatalogProvider(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; private set; }

        public bool IsAvailable()
        {
            return true;
        }

        public DataSet GetCatalogTypes()
        {
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(QUERY_TYPES, cnn))
                {
                    DataSet dataSet = new DataSet();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                    dataAdapter.Fill(dataSet, "CatalogTypes");
                    return dataSet;
                }
            }
        }

        public DataSet GetCatalogBrands()
        {
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(QUERY_GBRANDS, cnn))
                {
                    DataSet dataSet = new DataSet();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                    dataAdapter.Fill(dataSet, "CatalogBrands");
                    return dataSet;
                }
            }
        }

        public DataSet GetItemById(int id)
        {
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(QUERY_ITEMSBYID, cnn))
                {
                    SqlParameter param = new SqlParameter("id", id);
                    cmd.Parameters.Add(param);
                    DataSet dataSet = new DataSet();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                    dataAdapter.Fill(dataSet, "CatalogItems");
                    return dataSet;
                }
            }
        }

        public DataSet GetItems(int typeId = -1, int brandId = -1, string query = null)
        {
            SqlParameter paramType = null;
            SqlParameter paramBrand = null;
            SqlParameter paramQuery = null;

            string sqlQuery = QUERY_ITEMS;
            string sqlWhere = null;

            if (typeId > 0)
            {
                paramType = new SqlParameter("typeId", typeId);
                sqlWhere = "CatalogTypeId = @typeId";
            }

            if (brandId > 0)
            {
                paramBrand = new SqlParameter("brandId", brandId);
                sqlWhere = sqlWhere == null ? String.Empty : sqlWhere + " AND ";
                sqlWhere += "CatalogBrandId = @brandId";
            }

            if (!String.IsNullOrEmpty(query))
            {
                paramQuery = new SqlParameter("@query", String.Format("%{0}%", query));
                sqlWhere = sqlWhere == null ? String.Empty : sqlWhere + " AND ";
                sqlWhere += "Name LIKE @query";
            }

            sqlQuery = sqlWhere == null ? sqlQuery : sqlQuery + " WHERE " + sqlWhere;

            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    if (paramType != null)
                    {
                        cmd.Parameters.Add(paramType);
                    }
                    if (paramBrand != null)
                    {
                        cmd.Parameters.Add(paramBrand);
                    }
                    if (paramQuery != null)
                    {
                        cmd.Parameters.Add(paramQuery);
                    }
                    DataSet dataSet = new DataSet();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                    dataAdapter.Fill(dataSet, "CatalogItems");
                    return dataSet;
                }
            }
        }

        public DataSet GetDatasetSchema()
        {
            string sqlQuery = QUERY_ITEMS + " WHERE 1=0";

            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnn))
                {
                    DataSet dataSet = new DataSet();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                    dataAdapter.Fill(dataSet, "CatalogItems");
                    return dataSet;
                }
            }
        }

        public int CreateItem(DataSet dataSet)
        {
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(CREATE_ITEMS, cnn))
                {
                    cmd.Parameters.Add(new SqlParameter("Id", SqlDbType.Int) { SourceColumn = "Id", Direction = ParameterDirection.Output });
                    cmd.Parameters.Add(new SqlParameter("Name", SqlDbType.VarChar) { SourceColumn = "Name" });
                    cmd.Parameters.Add(new SqlParameter("Description", SqlDbType.VarChar) { SourceColumn = "Description" });
                    cmd.Parameters.Add(new SqlParameter("Price", SqlDbType.Decimal) { SourceColumn = "Price" });
                    cmd.Parameters.Add(new SqlParameter("CatalogTypeId", SqlDbType.Int) { SourceColumn = "CatalogTypeId" });
                    cmd.Parameters.Add(new SqlParameter("CatalogBrandId", SqlDbType.Int) { SourceColumn = "CatalogBrandId" });

                    SqlDataAdapter dataAdapter = new SqlDataAdapter();
                    dataAdapter.InsertCommand = cmd;
                    return dataAdapter.Update(dataSet, "CatalogItems");
                }
            }
        }

        public int UpdateItem(DataSet dataSet)
        {
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(UPDATE_ITEMS, cnn))
                {
                    cmd.Parameters.Add(new SqlParameter("Id", SqlDbType.Int) { SourceColumn = "Id" });
                    cmd.Parameters.Add(new SqlParameter("Name", SqlDbType.VarChar) { SourceColumn = "Name" });
                    cmd.Parameters.Add(new SqlParameter("Description", SqlDbType.VarChar) { SourceColumn = "Description" });
                    cmd.Parameters.Add(new SqlParameter("Price", SqlDbType.Decimal) { SourceColumn = "Price" });
                    cmd.Parameters.Add(new SqlParameter("CatalogTypeId", SqlDbType.Int) { SourceColumn = "CatalogTypeId" });
                    cmd.Parameters.Add(new SqlParameter("CatalogBrandId", SqlDbType.Int) { SourceColumn = "CatalogBrandId" });

                    SqlDataAdapter dataAdapter = new SqlDataAdapter();
                    dataAdapter.UpdateCommand = cmd;
                    return dataAdapter.Update(dataSet, "CatalogItems");
                }
            }
        }

        public int DeleteItem(int id)
        {
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(DELETE_ITEM, cnn))
                {
                    SqlParameter param = new SqlParameter("id", id);
                    cmd.Parameters.Add(param);
                    cnn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
