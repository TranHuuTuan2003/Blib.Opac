using System.Text;

using Dapper;

using KMS.Api.Infrastructure.DbContext.master;

using UC.Core.Models;
using UC.Core.Models.FormData;

namespace KMS.Api.Helpers
{
    public sealed class DataTableServerSideService
    {
        private readonly UnitOfWork _unitOfWork;
        public DataTableServerSideService(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        /// <summary>
        /// Logic Datatables server side.
        /// </summary>

        public async Task<PagedResponse> SearchAsync<T>(
            HttpContext httpContext,
            string selectFromSql,
            Func<FormData, (string whereSql, DynamicParameters param)> buildWhere,
            string cteSql = ""
        )
        {
            try
            {
                var form = new FormData(httpContext);
                var (where, param) = buildWhere(form);

                var columnOrder = form.GetColumnOrder();
                var col = columnOrder.data;
                var dir = columnOrder.dir;

                var countSql = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(cteSql))
                    countSql.AppendLine(cteSql);
                countSql.AppendLine($"SELECT COUNT(*) FROM ({selectFromSql} {where}) AS c");

                var total = await _unitOfWork.Repository.QueryFirstAsync<int>(countSql.ToString(), param);

                // Tính phân trang
                var paged = new PagedRequest(httpContext, total);
                var pageSize = paged.PageSize;
                var skip = paged.SkipRows;

                var finalSql = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(cteSql))
                    finalSql.AppendLine(cteSql);

                finalSql.AppendLine(selectFromSql);
                finalSql.AppendLine(where);
                finalSql.AppendLine($"ORDER BY {col} {dir}");
                finalSql.AppendLine("LIMIT @take OFFSET @skip");

                param.Add("@take", pageSize);
                param.Add("@skip", skip);

                var data = await _unitOfWork.Repository.QueryListAsync<T>(finalSql.ToString(), param);

                return new PagedResponse(paged.CurrentPage, total, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new PagedResponse(0, 0, Array.Empty<string>());
            }
        }
    }
}