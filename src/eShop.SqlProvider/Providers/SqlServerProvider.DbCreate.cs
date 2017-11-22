using System;
using System.IO;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace eShop.SqlProvider
{
    public partial class SqlServerProvider
    {
        const string CurrentVersion = "1.0";

        const string QUERY_EXISTSDB = "SELECT count(*) FROM sys.Databases WHERE name = @DbName";
        const string QUERY_VERSION = "SELECT [Current] FROM [Version]";

        public bool DatabaseExists()
        {
            SqlConnectionStringBuilder cnnStringBuilder = new SqlConnectionStringBuilder(ConnectionString);
            string dbName = cnnStringBuilder.InitialCatalog;
            cnnStringBuilder.InitialCatalog = "master";
            string masterConnectionString = cnnStringBuilder.ConnectionString;

            using (SqlConnection cnn = new SqlConnection(masterConnectionString))
            {
                cnn.Open();
                using (SqlCommand cmd = new SqlCommand(QUERY_EXISTSDB, cnn))
                {
                    SqlParameter param = new SqlParameter("DbName", dbName);
                    cmd.Parameters.Add(param);
                    return (int)cmd.ExecuteScalar() == 1;
                }
            }
        }

        public bool IsLastVersion()
        {
            return GetVersion() == CurrentVersion;
        }

        public string GetVersion()
        {
            try
            {
                using (SqlConnection cnn = new SqlConnection(ConnectionString))
                {
                    cnn.Open();
                    using (SqlCommand cmd = new SqlCommand(QUERY_VERSION, cnn))
                    {
                        return cmd.ExecuteScalar() as String;
                    }
                }
            }
            catch
            {
                return String.Empty;
            }
        }

        public void CreateDatabase()
        {
            SqlConnectionStringBuilder cnnStringBuilder = new SqlConnectionStringBuilder(ConnectionString);
            string dbName = cnnStringBuilder.InitialCatalog;
            if (dbName == null)
            {
                throw new ArgumentNullException("Initial Catalog");
            }
            if (dbName.Equals("master", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid Initial Catalog 'master'.");
            }
            cnnStringBuilder.InitialCatalog = "master";
            string masterConnectionString = cnnStringBuilder.ConnectionString;

            using (SqlConnection cnn = new SqlConnection(masterConnectionString))
            {
                cnn.Open();
                foreach (string sqlLine in GetSqlScriptLines(dbName))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlLine, cnn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                using (SqlCommand cmd = new SqlCommand($"INSERT INTO [Version] ([Current]) VALUES ('{CurrentVersion}')", cnn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private IEnumerable<string> GetSqlScriptLines(string dbName)
        {
            string sqlScript = GetSqlScript();
            sqlScript = sqlScript.Replace("[DATABASE_NAME]", dbName);
            using (var reader = new StringReader(sqlScript))
            {
                string sql = "";
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (line.Trim() == "GO")
                    {
                        yield return sql;
                        sql = "";
                    }
                    else
                    {
                        sql += line;
                    }
                    line = reader.ReadLine();
                }
            }
        }

        private string GetSqlScript()
        {
            Stream stream = System.Reflection.Assembly.GetCallingAssembly().GetManifestResourceStream("eShop.SqlProvider.SqlScripts.CreateDb.sql");
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
