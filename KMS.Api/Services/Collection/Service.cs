using Dapper;
using KMS.Api.Common;
using KMS.Api.Core;
using KMS.Api.Helpers;
using KMS.Api.Infrastructure.DbContext.master;
using KMS.Api.Models.Document;
using KMS.Api.Services.Search.Logic;
using KMS.Shared.DTOs.Document;
using KMS.Shared.DTOs.Search;
using KMS.Shared.Helpers;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using System.Text.Json;

namespace KMS.Api.Services.Collection
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
        private readonly IIntermediateSearchLogic _intermediateSearchLogic;

        public Service(
            UnitOfWork unitOfWork,
            ApiHelper apiHelper,
            AppConfigHelper appConfigHelper,
            ILogger<ServiceWrapper> logger,
            IMemoryCache memoryCache,
            IIntermediateSearchLogic intermediateSearchLogic
            )
        {
            _unitOfWork = unitOfWork;
            _apiHelper = apiHelper;
            _tenantCodes = appConfigHelper.GetTenantCodes();
            _appConfigHelper = appConfigHelper;
            _logger = logger;
            _enableQueryLog = appConfigHelper.GetEnableSqlQueryLog();
            _memoryCache = memoryCache;
            _intermediateSearchLogic = intermediateSearchLogic;
        }

        //public async Task<List<CollectionTree>> CollectionTreeDbTypeAsync(string dbType)
        //{
        //    if (string.IsNullOrEmpty(dbType))
        //    {
        //        throw new Exception("Data cannot be null or empty!");
        //    }

        //    if (dbType != "adoc" && dbType != "pdoc" && dbType != "ddoc" && dbType != "all")
        //    {
        //        return new();
        //    }

        //    StringBuilder sql = new StringBuilder();

        //    var lang = _appConfigHelper.GetLangFromContext();

        //    sql.AppendLine(lang == "en" ? ConstQuery.SelectCollectionTreeEn : ConstQuery.SelectCollectionTree);
        //    sql.AppendLine("FROM o_collection oc");
        //    if (dbType == "all")
        //    {
        //        sql.AppendLine("WHERE (target_url IS NULL OR target_url = '') AND isactive = TRUE AND isopac = TRUE");
        //    }
        //    else
        //    {
        //        sql.AppendLine("WHERE (target_url IS NULL OR target_url = '') AND (db_type = @dbType OR db_type = 'adoc') AND isactive = TRUE AND isopac = TRUE");
        //    }

        //    sql.AppendLine($"AND tenant_code = ANY(@_tenantCodes) ORDER BY order_index");

        //    var collections = await _unitOfWork.Repository.QueryListAsync<CollectionTree>(sql.ToString(), new { dbType, _tenantCodes });
        //    return TreeHelper.BuildTree(collections);
        //}


        //public async Task<SearchResponse> SearchingAsync(string type, int page, int pageSize, SearchBody searchRequest)
        //{
        //    string searchRequestJson = JsonSerializer.Serialize(searchRequest, new JsonSerializerOptions
        //    {
        //        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        //        WriteIndented = false
        //    });

        //    string hashKey = HashHelper.ComputeMD5Hash(searchRequestJson);
        //    string cacheKey = $"Search_{type}_{page}_{pageSize}_{hashKey}";

        //    bool enableCache = _appConfigHelper.GetEnableCacheResultSearch();
        //    int cacheInMinutes = _appConfigHelper.GetCacheResultSearchInMinute();
        //    int slidingCache = _appConfigHelper.GetSlidingCacheInMinute();

        //    try
        //    {
        //        if (!enableCache || !_memoryCache.TryGetValue(cacheKey, out SearchResponse? cachedResult))
        //        {
        //            cachedResult = type.ToLower() switch
        //            {
        //                "quick" => await QuickSearchingAsync(page, pageSize, searchRequest),
        //                "basic" => await BasicSearchingAsync(page, pageSize, searchRequest),
        //                "advance" => await AdvanceSearchingAsync(page, pageSize, searchRequest),
        //                _ => new()
        //            };

        //            // Chỉ lưu cache nếu EnableCacheResultSearch = true
        //            if (enableCache)
        //            {
        //                var cacheOptions = new MemoryCacheEntryOptions()
        //                {
        //                    SlidingExpiration = TimeSpan.FromMinutes(slidingCache),
        //                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheInMinutes)
        //                };
        //                _memoryCache.Set(cacheKey, cachedResult, cacheOptions);
        //            }
        //        }

        //        return cachedResult ?? new();
        //    }
        //    catch (Exception ex)
        //    {
        //        LoggerHelper.LogError(_logger, ex, ex.Message);
        //        return new();
        //    }
        //}

        //private async Task<SearchResponse> QuickSearchingAsync(int page, int pageSize, SearchBody model)
        //{
        //    page = page <= 0 ? 1 : page;
        //    pageSize = pageSize <= 0 ? 10 : pageSize;

        //    StringBuilder query = new StringBuilder();
        //    var (builtQuery, parameters) = _intermediateSearchLogic.QuickBuildQuerySearch(model, "collection");
        //    query.AppendLine(builtQuery);

        //    StringBuilder countQuery = new StringBuilder();
        //    countQuery.AppendLine(query.ToString().Replace(ConstQuery.SelectDocumentViewQuery, "SELECT COUNT(*)"));

        //    query.AppendLine(SearchQueryHelper.BuildQueryOrderAndOffset(page, pageSize, model));

        //    LoggerHelper.LogQuery(_enableQueryLog, _logger, "Query COLLECTION count QUICK search: {NewLine}{Sql}",
        //        Environment.NewLine, countQuery);
        //    int count = await _unitOfWork.Repository.QueryFirstAsync<int>(countQuery.ToString(), parameters);
        //    LoggerHelper.LogQuery(_enableQueryLog, _logger, "Query COLLECTION QUICK search: {Sql}", query);
        //    List<Result> items = await _unitOfWork.Repository.QueryListAsync<Result>(query.ToString(), parameters);
        //    var totalRecords = count;
        //    var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

        //    var skip = (page - 1) * pageSize;

        //    SearchResponse searchResult = new SearchResponse();

        //    searchResult.currentPage = page;
        //    searchResult.totalPages = totalPages;
        //    searchResult.pageSize = pageSize;
        //    searchResult.totalRecords = totalRecords;
        //    searchResult.results = items;

        //    return searchResult;
        //}

        //private async Task<SearchResponse> BasicSearchingAsync(int page, int pageSize, SearchBody model)
        //{
        //    page = page <= 0 ? 1 : page;
        //    pageSize = pageSize <= 0 ? 10 : pageSize;

        //    StringBuilder query = new StringBuilder();
        //    var (builtQuery, parameters) = _intermediateSearchLogic.BasicBuildQuerySearch(model, "collection");
        //    query.AppendLine(builtQuery);

        //    StringBuilder countQuery = new StringBuilder();
        //    countQuery.AppendLine(query.ToString().Replace(ConstQuery.SelectDocumentViewQuery, "SELECT COUNT(*)"));

        //    query.AppendLine(SearchQueryHelper.BuildQueryOrderAndOffset(page, pageSize, model));

        //    LoggerHelper.LogQuery(_enableQueryLog, _logger, "Query COLLECTION count BASIC search: {NewLine}{Sql}",
        //        Environment.NewLine, countQuery);
        //    int count = await _unitOfWork.Repository.QueryFirstAsync<int>(countQuery.ToString(), parameters);
        //    LoggerHelper.LogQuery(_enableQueryLog, _logger, "Query COLLECTION QUICK search: {NewLine}{Sql}",
        //        Environment.NewLine, query);
        //    List<Result> items = await _unitOfWork.Repository.QueryListAsync<Result>(query.ToString(), parameters);
        //    var totalRecords = count;
        //    var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

        //    var skip = (page - 1) * pageSize;

        //    SearchResponse searchResult = new SearchResponse();

        //    searchResult.currentPage = page;
        //    searchResult.totalPages = totalPages;
        //    searchResult.pageSize = pageSize;
        //    searchResult.totalRecords = totalRecords;
        //    searchResult.results = items;

        //    return searchResult;
        //}

        //private async Task<SearchResponse> AdvanceSearchingAsync(int page, int pageSize, SearchBody model)
        //{
        //    page = page <= 0 ? 1 : page;
        //    pageSize = pageSize <= 0 ? 10 : pageSize;

        //    StringBuilder query = new StringBuilder();
        //    var (builtQuery, parameters) = _intermediateSearchLogic.AdvanceBuildQuerySearch(model, "collection");
        //    query.AppendLine(builtQuery);

        //    StringBuilder countQuery = new StringBuilder();
        //    countQuery.AppendLine(query.ToString().Replace(ConstQuery.SelectDocumentViewQuery, "SELECT COUNT(*)"));

        //    query.AppendLine(SearchQueryHelper.BuildQueryOrderAndOffset(page, pageSize, model));

        //    LoggerHelper.LogQuery(_enableQueryLog, _logger, "Query COLLECTION count ADVANCE search: {NewLine}{Sql}",
        //        Environment.NewLine, countQuery);
        //    int count = await _unitOfWork.Repository.QueryFirstAsync<int>(countQuery.ToString(), parameters);
        //    LoggerHelper.LogQuery(_enableQueryLog, _logger, "Query COLLECTION ADVANCE search: {NewLine}{Sql}",
        //        Environment.NewLine, query);
        //    List<Result> items = await _unitOfWork.Repository.QueryListAsync<Result>(query.ToString(), parameters);
        //    var totalRecords = count;
        //    var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

        //    var skip = (page - 1) * pageSize;

        //    SearchResponse searchResult = new SearchResponse();

        //    searchResult.currentPage = page;
        //    searchResult.totalPages = totalPages;
        //    searchResult.pageSize = pageSize;
        //    searchResult.totalRecords = totalRecords;
        //    searchResult.results = items;

        //    return searchResult;
        //}

    }
}
