using Dapper;
using System.Data;
using UC.Core.Abstracts;
using UC.Core.Models;

namespace KMS.Api.Infrastructure.DbContext.slave
{
    public class UnitOfWorkBlib : AbsUnitOfWork<DbSessionBlib>
    {
        public UnitOfWorkBlib(DbSessionBlib session) : base(session)
        {

        }
    }
}
