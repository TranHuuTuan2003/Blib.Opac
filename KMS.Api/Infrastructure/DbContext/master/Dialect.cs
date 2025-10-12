using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Infrastructure/DbContext/master/Dialect.cs
namespace KMS.Api.Infrastructure.DbContext.master
{
    public enum SqlDialect { PostgreSql, SqlServer, Oracle }

    public static class Dialect
    {
        public static SqlDialect Resolve(IConfiguration cfg)
        {
            var v = cfg["ConnectionStrings:master_dbtype"];
            return v switch
            {
                "PostgreSql" => SqlDialect.PostgreSql,
                "SqlServer"  => SqlDialect.SqlServer,
                "Oracle"     => SqlDialect.Oracle,
                _ => SqlDialect.PostgreSql
            };
        }
        public static string Q(string ident, SqlDialect d) =>
            d switch
            {
                SqlDialect.PostgreSql => $"\"{ident}\"",
                SqlDialect.SqlServer  => $"[{ident}]",
                SqlDialect.Oracle     => ident,
                _ => ident
            };
    }
}
