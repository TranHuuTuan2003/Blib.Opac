using Dapper;
using KMS.Api.Common;
using KMS.Api.Core;
using KMS.Api.Helpers;
using KMS.Api.Infrastructure.DbContext.master;
using KMS.Api.Infrastructure.DbContext.slave;
using KMS.Api.Models.Document;
using KMS.Api.Services.Search.Logic;
using KMS.Shared.DTOs.Document;
using KMS.Shared.DTOs.Search;
using KMS.Shared.DTOs.Tree;
using KMS.Shared.Helpers;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using System.Text.Json;

namespace KMS.Api.Services.TrainingProgram
{
    public class Service : IService
    {
        private readonly UnitOfWorkBlib _unitOfWorkBlib;
        private readonly ApiHelper _apiHelper;
        private readonly AppConfigHelper _appConfigHelper;
        private readonly ILogger<ServiceWrapper> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IReadOnlyList<string> _tenantCodes;
        private readonly bool _enableQueryLog;
        private readonly IIntermediateSearchLogic _intermediateSearchLogic;

        public Service(
            UnitOfWorkBlib unitOfWorkBlib,
            ApiHelper apiHelper,
            AppConfigHelper appConfigHelper,
            ILogger<ServiceWrapper> logger,
            IMemoryCache memoryCache,
            IIntermediateSearchLogic intermediateSearchLogic
            )
        {
            _unitOfWorkBlib = unitOfWorkBlib;
            _apiHelper = apiHelper;
            _tenantCodes = appConfigHelper.GetTenantCodes();
            _appConfigHelper = appConfigHelper;
            _logger = logger;
            _enableQueryLog = appConfigHelper.GetEnableSqlQueryLog();
            _memoryCache = memoryCache;
            _intermediateSearchLogic = intermediateSearchLogic;
        }

        public async Task<List<object>> GetTrainingProgram()
        {
            var sql = "SELECT name, id FROM edu.aca_system ORDER BY order_index ASC limit 10";
            return await _unitOfWorkBlib.Repository.QueryListAsync<object>(sql, null);
        }

    }
}
