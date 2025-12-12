using Microsoft.Data.SqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using UC.Core.Abstracts;

namespace KMS.Api.Infrastructure.DbContext.slave
{
    public class DbSessionBlib : absBaseSession, IDisposable
    {
        public DbSessionBlib(IConfiguration configuration)
        {
            if (configuration.GetConnectionString("slave") != null && !string.IsNullOrEmpty(configuration.GetConnectionString("master").ToString()))
            {
                if (configuration.GetSection("ConnectionStrings:slave_dbtype").Value == "PostgreSql")
                {
                    Connection = new NpgsqlConnection(configuration.GetConnectionString("slave"));
                }
                else if (configuration.GetSection("ConnectionStrings:slave_dbtype").Value == "SqlServer")
                {
                    Connection = new SqlConnection(configuration.GetConnectionString("slave"));
                }
                else if (configuration.GetSection("ConnectionStrings:slave_dbtype").Value == "Oracle")
                {
                    Connection = new OracleConnection(configuration.GetConnectionString("slave"));
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
