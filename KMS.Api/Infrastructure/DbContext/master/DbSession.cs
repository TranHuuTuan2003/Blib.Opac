using Microsoft.Data.SqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using UC.Core.Abstracts;

namespace KMS.Api.Infrastructure.DbContext.master
{
    public class DbSession : absBaseSession, IDisposable
    {
        public DbSession(IConfiguration configuration)
        {
            if (configuration.GetConnectionString("master") != null && !string.IsNullOrEmpty(configuration.GetConnectionString("master").ToString()))
            {
                if (configuration.GetSection("ConnectionStrings:master_dbtype").Value == "PostgreSql")
                {
                    Connection = new NpgsqlConnection(configuration.GetConnectionString("master"));
                }
                else if (configuration.GetSection("ConnectionStrings:master_dbtype").Value == "SqlServer")
                {
                    Connection = new SqlConnection(configuration.GetConnectionString("master"));
                }
                else if (configuration.GetSection("ConnectionStrings:master_dbtype").Value == "Oracle")
                {
                    Connection = new OracleConnection(configuration.GetConnectionString("master"));
                }
                Connection.Open();
            }
        }

        public void Dispose()
        {
            if (Connection != null && Connection.State == ConnectionState.Open)
            {
                Connection.Close();
                Connection.Dispose();
            }
        }
    }
}
