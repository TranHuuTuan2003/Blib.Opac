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
        private readonly UnitOfWork _unitOfWork;
        private readonly ApiHelper _apiHelper;
        private readonly AppConfigHelper _appConfigHelper;
        private readonly ILogger<ServiceWrapper> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IReadOnlyList<string> _tenantCodes;
        private readonly bool _enableQueryLog;
        private readonly IIntermediateSearchLogic _intermediateSearchLogic;

        public Service(
            UnitOfWorkBlib unitOfWorkBlib,
            UnitOfWork unitOfWork,
            ApiHelper apiHelper,
            AppConfigHelper appConfigHelper,
            ILogger<ServiceWrapper> logger,
            IMemoryCache memoryCache,
            IIntermediateSearchLogic intermediateSearchLogic
            )
        {
            _unitOfWorkBlib = unitOfWorkBlib;
            _unitOfWork = unitOfWork;
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
                    ase.id,
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
                    ase.id,
                    ase.code,
                    ase.name,
                    ase.no_of_cert;
            ";

            return await _unitOfWorkBlib.Repository.QueryListAsync<ListSection>(sql, new {id});
        }
        public async Task<List<ListSection>> GetListSectionTrue(string id)
        {
            var sql = @"SELECT
                    ase.id,
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
                    ase.id,
                    ase.code,
                    ase.name,
                    ase.no_of_cert;
            ";

            return await _unitOfWorkBlib.Repository.QueryListAsync<ListSection>(sql, new { id });
        }


        public async Task<List<DetailSection>> GetDetailSection(string id)
        {
            var sql = @"select code,name, department_id ,no_of_cert from edu.aca_section where id = @id ";

            return await _unitOfWorkBlib.Repository.QueryListAsync<DetailSection>(sql, new { id });
        }

        public async Task<List<ListSubject>> GetListSubject(string sectionId)
        {
            var sqlSubject = @"
                SELECT 
                    asu.id,
                    asu.title,
                    asu.bib_author,
                    asu.bib_publisher,
                    asu.bib_publishplace AS bib_publisplace,
                    asu.bib_yearpub
                FROM edu.aca_subject asu
                JOIN edu.aca_section_subject ass 
                    ON ass.subject_id = asu.id
                WHERE ass.section_id = @sectionId
            ";

            var subjects = await _unitOfWorkBlib.Repository
                .QueryListAsync<ListSubject>(sqlSubject, new { sectionId });

            if (subjects == null || subjects.Count == 0)
                return new List<ListSubject>();

            foreach (var subject in subjects)
            {
                subject.ListSubjectLib = await GetSubjectLibBySubjectId(subject.id);
            }

            return subjects;
        }

        private async Task<List<ListSubjectLib>> GetSubjectLibBySubjectId(string subjectId)
        {
            var sqlLib = @"
                SELECT 
                            bib_id,
                            title,
                            bib_author,
                            bib_publisher,
                            bib_publishplace AS bib_publisplace,
                            bib_yearpub,
                            is_mandatory_ref,
                            is_optional_ref,
                            is_included_ref
                        FROM edu.aca_subject_lib
                        WHERE subject_id = @subjectId
            ";

            var libs = (await _unitOfWorkBlib.Repository
                .QueryListAsync<ListSubjectLib>(sqlLib, new { subjectId }))
                .ToList();

            if (!libs.Any())
                return libs;

            var bibIds = libs.Select(x => x.bib_id).Distinct().ToArray();

            var sqlSlug = @"
                SELECT 
                    mfn AS BibId,
                    slug
                FROM o_item
                WHERE mfn = ANY(@bibIds)
            ";

            var slugMap = (await _unitOfWork.Repository
                .QueryListAsync<(int BibId, string Slug)>(sqlSlug, new { bibIds }))
                .ToDictionary(x => x.BibId, x => x.Slug);

            foreach (var lib in libs)
            {
                if (slugMap.TryGetValue(lib.bib_id, out var slug))
                {
                    lib.slug = slug;
                }
            }

            return libs;
        }


    }
}
