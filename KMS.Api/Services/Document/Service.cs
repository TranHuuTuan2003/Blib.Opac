using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Caching.Memory;

using Dapper;

using KMS.Api.Common;
using KMS.Api.Core;
using KMS.Api.Helpers;
using KMS.Api.Infrastructure.DbContext.master;
using KMS.Api.Models.Document;
using KMS.Shared.DTOs.Document;
using KMS.Shared.Helpers;

namespace KMS.Api.Services.Document
{
    public class Service : IService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ApiHelper _apiHelper;
        private readonly AppConfigHelper _appConfigHelper;
        private readonly ILogger<ServiceWrapper> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IReadOnlyList<string> _tenantCodes;
        private readonly bool _enableQueryLog;

        public Service(
            UnitOfWork unitOfWork,
            ApiHelper apiHelper,
            AppConfigHelper appConfigHelper,
            ILogger<ServiceWrapper> logger,
            IMemoryCache memoryCache
            )
        {
            _unitOfWork = unitOfWork;
            _apiHelper = apiHelper;
            _tenantCodes = appConfigHelper.GetTenantCodes();
            _appConfigHelper = appConfigHelper;
            _logger = logger;
            _enableQueryLog = appConfigHelper.GetEnableSqlQueryLog();
            _memoryCache = memoryCache;
        }

        public async Task<Details> GetDetailAsync(string slug)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(ConstQuery.SelectDocumentDetailQuery);
            sql.AppendLine("FROM o_item oi");
            sql.AppendLine("LEFT JOIN o_item_detail oid2 on oi.id = oid2.id");
            sql.AppendLine($"WHERE oi.slug = @slug AND is_lock = false AND oi.tenant_code = ANY(@_tenantCodes)");

            LoggerHelper.LogQuery(_enableQueryLog, _logger, "Query get DOCUMENT DETAILS: {NewLine}{Sql}", Environment.NewLine, sql.ToString());
            DocumentMapper? detail = await _unitOfWork.Repository.QueryFirstAsync<DocumentMapper?>(sql.ToString(), new { slug, _tenantCodes });
            List<DublinCoreField>? dublinCoreObject = null;
            List<MarcField>? marcObject = null;

            if (detail != null)
            {
                try
                {
                    if (detail.db_type == "pdoc" || detail.db_type == "adoc")
                    {
                        if (!string.IsNullOrEmpty(detail?.marc_field_value))
                        {
                            marcObject = JsonSerializer.Deserialize<List<MarcField>>(detail.marc_field_value);
                        }
                    }

                    if (detail?.db_type == "ddoc" || detail?.db_type == "adoc")
                    {
                        if (!string.IsNullOrEmpty(detail?.dublin_core))
                        {
                            dublinCoreObject = JsonSerializer.Deserialize<List<DublinCoreField>>(detail.dublin_core);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoggerHelper.LogError(_logger, ex, "Lỗi khi parse JSON cho tài liệu. Slug: {slug}, error: {error}", slug, ex.Message);
                }

                var documentDetail = new Details();
                ModelMapper.MapProperties(detail, documentDetail);
                documentDetail.marc_field_value_object = marcObject;
                documentDetail.dublin_core_object = dublinCoreObject;

                // var paramConfig = _configService.Get();
                // if (paramConfig.AlwaysShowHoldingRegCir)
                // {
                //     if (detail != null && detail.mfn != null)
                //     {
                //         documentDetail.register_circulation_place = await GetRegisterCirAsync(detail.mfn.Value);
                //     }
                // }

                return documentDetail;
            }

            return new();
        }

        public async Task<string?> GetSlugAsync(int? mfn, int? did)
        {
            if (mfn != null)
            {
                return await _unitOfWork.Repository.QueryFirstAsync<string?>("SELECT slug FROM o_item WHERE mfn = @mfn", new { mfn });
            }
            else if (did != null)
            {
                return await _unitOfWork.Repository.QueryFirstAsync<string?>("SELECT slug FROM o_item WHERE did = @did", new { did });
            }
            throw new Exception();
        }

        public async Task<List<Borrowing>> GetDocsBorrowingAsync(string card_no)
        {
            if (string.IsNullOrEmpty(card_no))
            {
                throw new BusinessException("Số thẻ của bạn đọc không hợp lệ!");
            }

            var apiUrl = _appConfigHelper.GetApiBlib();
            var url = apiUrl + "Reader/borrowing?card_no=" + card_no;
            var response = await _apiHelper.GetApiResponseAsync<List<Borrowing>>(url);
            if (response.Data != null)
            {
                return response.Data;
            }

            return new();
        }

        public async Task<List<Extending>> GetDocsExtendAsync(string card_no)
        {
            if (string.IsNullOrEmpty(card_no))
            {
                throw new BusinessException("Số thẻ của bạn đọc không hợp lệ!");
            }

            var apiUrl = _appConfigHelper.GetApiBlib();
            var url = apiUrl + "Reader/extend?card_no=" + card_no;
            var response = await _apiHelper.GetApiResponseAsync<List<Extending>>(url);
            if (response.Data != null)
            {
                return response.Data;
            }

            return new();
        }

        public async Task<List<Returned>> GetDocsReturnedAsync(string card_no)
        {
            if (string.IsNullOrEmpty(card_no))
            {
                throw new BusinessException("Số thẻ của bạn đọc không hợp lệ!");
            }

            var apiUrl = _appConfigHelper.GetApiBlib();
            var url = apiUrl + "Reader/returned?card_no=" + card_no;
            var response = await _apiHelper.GetApiResponseAsync<List<Returned>>(url);
            if (response.Data != null)
            {
                return response.Data;
            }

            return new();
        }

        public async Task<string> GetMarcByMfn(int mfn)
        {
            var sql = "SELECT marc FROM o_item_detail WHERE mfn = @mfn";
            var marc = await _unitOfWork.Repository.QueryFirstAsync<string?>(sql, new { mfn });
            if (string.IsNullOrEmpty(marc)) throw new BusinessException("Không tìm thấy file marc của tài liệu!");
            return marc;
        }

        // public async Task<DocMarc21> GetMarc21Async(string slug)
        // {
        //     try
        //     {
        //         StringBuilder getIdSql = new StringBuilder();
        //         getIdSql.AppendLine($"SELECT id FROM o_item WHERE slug = @slug AND tenant_code = ANY(@_tenantCodes)");
        //         int id = await _unitOfWork.Repository.QueryFirstAsync<int>(getIdSql.ToString(), new { slug = slug });
        //         StringBuilder sql = new StringBuilder();
        //         sql.AppendLine(ConstQuery.SelectMarc21Query);
        //         sql.AppendLine("FROM o_item_detail oid2");
        //         sql.AppendLine($"WHERE oid2.id = @id AND oid2.tenant_code = ANY(@_tenantCodes)");

        //         var marc = await _unitOfWork.Repository.QueryFirstAsync<DocMarc21>(sql.ToString(), new { id = id });
        //         if (marc != null)
        //         {
        //             return marc;
        //         }

        //         return new DocMarc21();
        //     }
        //     catch (Exception ex)
        //     {
        //         _unitOfWork.Repository.Rollback();
        //         throw new CustomException<DocMarc21>(ex.Message);
        //     }
        // }

        // public async Task<DocDublinCore> GetDublinCoreAsync(string slug)
        // {
        //     try
        //     {
        //         StringBuilder getIdSql = new StringBuilder();
        //         getIdSql.AppendLine($"SELECT id FROM o_item WHERE slug = @slug AND tenant_code = ANY(@_tenantCodes)");
        //         int id = await _unitOfWork.Repository.QueryFirstAsync<int>(getIdSql.ToString(), new { slug = slug });
        //         StringBuilder sql = new StringBuilder();
        //         sql.AppendLine(ConstQuery.SelectDublinCoreQuery);
        //         sql.AppendLine("FROM o_item_detail oid2");
        //         sql.AppendLine($"WHERE oid2.id = @id AND oid2.tenant_code = ANY(@_tenantCodes)");
        //         var item = await _unitOfWork.Repository.QueryFirstAsync<DocDublinCore>(sql.ToString(), new { id = id });
        //         if (item != null)
        //         {
        //             return item;
        //         }

        //         return new DocDublinCore();
        //     }
        //     catch (Exception ex)
        //     {
        //         _unitOfWork.Repository.Rollback();
        //         throw new CustomException<DocDublinCore>(ex.Message);
        //     }
        // }

        private string BuildRegisterCirculationQuery()
        {
            var sql = new StringBuilder();
            sql.AppendLine("SELECT");
            sql.AppendLine("    register.id AS registername,");
            sql.AppendLine("    bib_id AS bibid,");
            sql.AppendLine("    register.status AS statuscode,");
            sql.AppendLine("    register_status.comment_status AS statusname,");
            sql.AppendLine("    c_circulation_place.name AS circulationplace,");
            sql.AppendLine("    c_circulation_place.id AS circulationid");
            sql.AppendLine("FROM register");
            sql.AppendLine("LEFT JOIN register_status ON register_status.status = register.status");
            sql.AppendLine("LEFT JOIN store ON store.id = register.store_id");
            sql.AppendLine("LEFT JOIN c_circulation_place ON c_circulation_place.id = store.cir_place_id");
            sql.AppendLine("WHERE 1=1");
            sql.AppendLine("AND COALESCE(lock_opac,'') != '1'");
            sql.AppendLine("AND bib_id = @id;");
            return sql.ToString();
        }

        // public async Task<List<ResgisteredCirculation>> GetRegisterCirAsync(int mfn)
        // {
        //     var mergeRegister = _appConfigHelper.GetEnableMergeRegisterIntoOpac();

        //     if (mergeRegister)
        //     {
        //         var sql = BuildRegisterCirculationQuery();
        //         LoggerHelper.LogQuery(_enableQueryLog, _logger, "Query get REGISTER CIRCULATION PLACE: {NewLine}{Sql}", Environment.NewLine, sql);
        //         var items = await _unitOfWork.Repository.QueryListAsync<ResgisteredCirculation>(sql, new { id = mfn });
        //         return items ?? new();
        //     }
        //     else
        //     {
        //         string baseUrl = _appConfigHelper.GetApiBlib();
        //         string url = baseUrl + "Register/get-register-circulation-opac/" + mfn;

        //         var response = await _apiHelper.GetApiResponseAsync<List<ResgisteredCirculation>>(url);
        //         if (response.Data != null)
        //         {
        //             return response.Data;
        //         }
        //         return new();
        //     }
        // }

        public async Task<List<Result>> GetRelatedDocuments(string slug, int limit)
        {
            string hashKey = HashHelper.ComputeMD5Hash(slug);
            string cacheKey = $"Related_Document_{hashKey}";

            bool enableCache = _appConfigHelper.GetEnableCacheResultSearch();
            int cacheInMinutes = _appConfigHelper.GetCacheResultSearchInMinute();
            int slidingCache = _appConfigHelper.GetSlidingCacheInMinute();

            if (!enableCache || !_memoryCache.TryGetValue(cacheKey, out List<Result>? cachedResult))
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("WITH target_doc AS (");
                sql.AppendLine("  SELECT titles, keywords");
                sql.AppendLine("  FROM o_item ");
                sql.AppendLine("  WHERE slug = @slug");
                sql.AppendLine(")");
                sql.AppendLine(ConstQuery.SelectRelatedDocumentQuery);
                sql.AppendLine(",similarity(oi.titles::text, td.titles::text) AS relevance_score");
                sql.AppendLine(",similarity(oi.keywords::text, td.keywords::text) AS relevance_score_2");
                sql.AppendLine("FROM o_item oi, target_doc td");
                sql.AppendLine("WHERE oi.title IS NOT NULL AND oi.title <> ''");
                sql.AppendLine("AND is_lock = false AND oi.tenant_code = ANY(@_tenantCodes)");
                sql.AppendLine("AND oi.slug != @slug");
                sql.AppendLine("ORDER BY relevance_score_2 DESC, relevance_score DESC");
                sql.AppendLine("LIMIT @limit");
                LoggerHelper.LogQuery(_enableQueryLog, _logger, "Query get RELATED DOCUMENT DETAILS: {NewLine}{Sql}", Environment.NewLine, sql.ToString());
                cachedResult = await _unitOfWork.Repository.QueryListAsync<Result>(sql.ToString(), new { slug, limit, _tenantCodes });

                if (enableCache)
                {
                    var cacheOptions = new MemoryCacheEntryOptions()
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(slidingCache),
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheInMinutes)
                    };
                    _memoryCache.Set(cacheKey, cachedResult, cacheOptions);
                }
            }

            return cachedResult ?? new();
        }

        public async Task UpdateDocumentView(string slug)
        {
            await TransactionHelper.ExecuteAsync(_unitOfWork, async () =>
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine($"UPDATE o_item SET view = COALESCE(view, 0) + 1 WHERE slug = @slug AND tenant_code = ANY(@_tenantCodes)");
                await _unitOfWork.Repository.ExecuteAsync(sql.ToString(), new { slug, _tenantCodes });
            });
        }

        public async Task SyncDocumentViews(List<SyncView> docViews)
        {
            await TransactionHelper.ExecuteAsync(_unitOfWork, async () =>
            {
                var sql = new StringBuilder();
                var parameters = new DynamicParameters();

                foreach (var docView in docViews)
                {
                    var idKey = "id_" + Guid.NewGuid().ToString("N")[..6];
                    var viewKey = "view_" + Guid.NewGuid().ToString("N")[..6];
                    sql.AppendLine($"UPDATE o_item SET view = COALESCE(view, 0) + @{viewKey} WHERE id = @{idKey} AND tenant_code = ANY(@_tenantCodes);");
                    parameters.Add(idKey, docView.id);
                    parameters.Add(viewKey, docView.view);
                }
                parameters.Add("_tenantCodes", _tenantCodes);

                await _unitOfWork.Repository.ExecuteAsync(sql.ToString(), parameters);
            });
        }

        public async Task<List<object>> GetTop12BibNew()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine($"select bib_type,slug,cover_photo,title,id from o_item order by created_date desc limit 12");
            var item = await _unitOfWork.Repository.QueryListAsync<object>(sql.ToString(),null);
            return item ?? new List<object>();
        }

        public async Task<List<CollectionDto>> GetTopBibCollection()
        {
            var sqlCollection = @"
            SELECT id, title, order_index
            FROM public.o_collection
            WHERE ishome = true AND isactive = true
            ORDER BY order_index ASC
            LIMIT 5;
             ";

            var collections = await _unitOfWork.Repository.QueryListAsync<dynamic>(sqlCollection, null);

            var result = new List<CollectionDto>();

            foreach (var col in collections)
            {
                string collectionId = col.id;

                var sqlItems = $@"
                    WITH RECURSIVE collection_hierarchy AS (
                        SELECT id, parent_id, title
                        FROM public.o_collection
                        WHERE id = '{collectionId}'
                        UNION ALL
                        SELECT c.id, c.parent_id, c.title
                        FROM public.o_collection c
                        INNER JOIN collection_hierarchy ch ON c.parent_id = ch.id
                    )
                    SELECT oi.id as item_id, oi.title as item_title, oi.slug, oi.cover_photo, oi.year_pub,oi.bib_type
                    FROM public.o_collection_item i
                    JOIN collection_hierarchy ch ON i.collection_id = ch.id
                    JOIN public.o_item oi ON oi.id = i.item_id
                    LIMIT 20;
                ";

                var items = await _unitOfWork.Repository.QueryListAsync<dynamic>(sqlItems, null);

                var dto = new CollectionDto
                {
                    CollectionId = (string)col.id,
                    CollectionTitle = (string)col.title,
                    Items = items.Select(x => new ItemDto
                    {
                        Title = (string)x.item_title,
                        Slug = (string)x.slug,
                        CoverPhoto = (string)x.cover_photo,
                        YearPub = (string)x.year_pub,
                         BibType = (string)x.bib_type
                    }).ToList()
                };


                result.Add(dto);
            }

            return result;
        }

    }
}
