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
using KMS.Shared.DTOs.TrainingProgram;
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

        public async Task<List<DetailSystem>> GetDetailSystem(string id)
        {
            var sql = @"select code,name,description, (select name from edu.aca_system  where parent_id = @id) as industry from edu.aca_system 
                WHERE id = @id 
            ";

            return await _unitOfWorkBlib.Repository.QueryListAsync<DetailSystem>(sql, new { id });
        }
        public async Task<List<ListSection>> GetListSectionFalse(string id)
        {
            var sql = @"SELECT
                    ase.code,
                    ase.name,
                    ase.no_of_cert,
                    COUNT(DISTINCT t.id)  AS subject_count,
                    COUNT(DISTINCT asl.id) AS subject_lib_count
                FROM edu.aca_section ase
                LEFT JOIN edu.aca_section_subject ass 
                    ON ass.section_id = ase.id
                LEFT JOIN edu.aca_system_section ass2  
                    ON ass2.section_id = ase.id
                LEFT JOIN edu.aca_subject t  
                    ON t.id = ass.subject_id
                LEFT JOIN edu.aca_subject_lib asl  
                    ON asl.subject_id = t.id
                WHERE ass2.system_id = @id and ase.is_different_majors = false
                GROUP BY
                    ase.code,
                    ase.name,
                    ase.no_of_cert;
            ";

            return await _unitOfWorkBlib.Repository.QueryListAsync<ListSection>(sql, new {id});
        }
        public async Task<List<ListSection>> GetListSectionTrue(string id)
        {
            var sql = @"SELECT
                    ase.code,
                    ase.name,
                    ase.no_of_cert,
                    COUNT(DISTINCT t.id)  AS subject_count,
                    COUNT(DISTINCT asl.id) AS subject_lib_count
                FROM edu.aca_section ase
                LEFT JOIN edu.aca_section_subject ass 
                    ON ass.section_id = ase.id
                LEFT JOIN edu.aca_system_section ass2  
                    ON ass2.section_id = ase.id
                LEFT JOIN edu.aca_subject t  
                    ON t.id = ass.subject_id
                LEFT JOIN edu.aca_subject_lib asl  
                    ON asl.subject_id = t.id
                WHERE ass2.system_id = @id and ase.is_different_majors = true
                GROUP BY
                    ase.code,
                    ase.name,
                    ase.no_of_cert;
            ";

            return await _unitOfWorkBlib.Repository.QueryListAsync<ListSection>(sql, new { id });
        }

    }
}
