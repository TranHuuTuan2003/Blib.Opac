using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Caching.Memory;

using Dapper;

using KMS.Api.Core;
using KMS.Api.Helpers;
using KMS.Api.Infrastructure.DbContext.master;
using KMS.Api.Services.Search.Logic;
using KMS.Shared.DTOs.Document;
using KMS.Shared.DTOs.FacetFilter;
using KMS.Shared.DTOs.Search;
using KMS.Shared.Helpers;

namespace KMS.Api.Services.Search
{
    public class Service : IService
    {
        private readonly IReadOnlyList<string> _tenantCodes;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMemoryCache _memoryCache;
        private readonly IIntermediateSearchLogic _intermediateSearchLogic;
        private readonly AppConfigHelper _appConfigHelper;
        private readonly ILogger<ServiceWrapper> _logger;
        private readonly bool _enableQueryLog;

        public Service(
            UnitOfWork unitOfWork,
            AppConfigHelper appConfigHelper,
            IIntermediateSearchLogic intermediateSearchLogic,
            IMemoryCache memoryCache,
            ILogger<ServiceWrapper> logger
            )
        {
            _unitOfWork = unitOfWork;
            _memoryCache = memoryCache;
            _intermediateSearchLogic = intermediateSearchLogic;
            _appConfigHelper = appConfigHelper;
            _logger = logger;
            _tenantCodes = appConfigHelper.GetTenantCodes();
            _enableQueryLog = appConfigHelper.GetEnableSqlQueryLog();
        }

        private async Task<List<FacetFilterResponse>> QueryFacetFilterAsync(StringBuilder query, FacetFilterRequest model, DynamicParameters parameters)
        {
            string fullQuery = query.ToString();

            int selectIndex = fullQuery.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase);
            int fromIndex = fullQuery.IndexOf("FROM", selectIndex, StringComparison.OrdinalIgnoreCase);

            if (selectIndex != -1 && fromIndex != -1)
            {
                fullQuery = "SELECT oi.id " + fullQuery.Substring(fromIndex);
            }

            query.Clear();
            query.Append(fullQuery);

            return await FacetFilterAsync(model, query.ToString(), parameters);
        }

        private async Task<FacetFilterResponse?> GetGroupOfFacetFilterItem(string code, string sql, DynamicParameters parameters, int page = 1, int pageSize = 5)
        {
            int offset = (page - 1) * pageSize;
            string? sqlCount = code switch
            {
                "bt" =>
                    $"select count(*) subcount, value from o_bibtype o where exists (select null from ({sql}) as a where a.id = o.item_id and o.tenant_code = ANY(@_tenantCodes)) and o.tenant_code = ANY(@_tenantCodes) and value is not null and value <> '' group by value order by subcount desc",

                "au" =>
                    $"select count(*) subcount, value from o_author o where exists (select null from ({sql}) as a where a.id = o.item_id and o.tenant_code = ANY(@_tenantCodes)) and o.tenant_code = ANY(@_tenantCodes) and value is not null and value <> '' group by value order by subcount desc",

                "kw" =>
                    $"select count(*) subcount, value from o_keyword o where exists (select null from ({sql}) as a where a.id = o.item_id and o.tenant_code = ANY(@_tenantCodes)) and o.tenant_code = ANY(@_tenantCodes) and value is not null and value <> '' group by value order by subcount desc",

                "yr" =>
                    $"select count(*) subcount, value from o_pubyear o where exists (select null from ({sql}) as a where a.id = o.item_id and o.tenant_code = ANY(@_tenantCodes)) and o.tenant_code = ANY(@_tenantCodes) and value is not null and value <> '' group by value order by value desc",

                "su" =>
                    $"select count(*) subcount, value from o_subject o where exists (select null from ({sql}) as a where a.id = o.item_id and o.tenant_code = ANY(@_tenantCodes)) and o.tenant_code = ANY(@_tenantCodes) and value is not null and value <> '' group by value order by subcount desc",

                "tn" =>
                    $"select count(*) subcount, tenant_code value from o_item o where id in ({sql}) group by tenant_code",

                _ => string.Empty
            };

            sqlCount += $" limit {pageSize} offset {offset}";

            var facetFilter = new FacetFilterResponse();
            facetFilter.code = code;
            try
            {
                LoggerHelper.LogQuery(_enableQueryLog, _logger, "Query facet filter: {NewLine}{Sql}", Environment.NewLine, sqlCount);
                List<FacetFilterResult> filters = await _unitOfWork.Repository.QueryListAsync<FacetFilterResult>(sqlCount, parameters);
                foreach (var item in filters)
                {
                    item.label = item.value;
                    facetFilter.rs.Add(item);
                }

                if (facetFilter.rs != null && facetFilter.rs.Count > 0)
                {
                    facetFilter.page = page;
                    facetFilter.pageSize = pageSize;
                    return facetFilter;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting facet filter item: {error}", ex.Message);
                _unitOfWork.Repository.Rollback();
                return new();
            }
        }

        private async Task<List<FacetFilterResponse>> FacetFilterAsync(FacetFilterRequest model, string sql, DynamicParameters parameters)
        {
            List<FacetFilterResponse> facetFilters = new();
            var tenantCodes = SearchQueryHelper.GetTenantFromFilterBy(model.searchRequest.filterBy);

            if (tenantCodes.Any())
            {
                parameters.Add("_tenantCodes", tenantCodes);
            }
            else
            {
                parameters.Add("_tenantCodes", _tenantCodes);
            }

            FacetFilterResponse? facetFilter;
            // code empty => init facet filter
            if (string.IsNullOrEmpty(model?.paging?.code))
            {
                for (int i = 0; i < model?.codes.Length; i++)
                {
                    facetFilter = await GetGroupOfFacetFilterItem(model.codes[i], sql, parameters);
                    if (facetFilter?.rs != null && facetFilter.rs.Count > 0)
                    {
                        facetFilter.forPaging = false;
                        facetFilters.Add(facetFilter);
                    }
                }
            }
            else
            {
                int page = model.paging.page == null ? 1 : model.paging.page.Value;
                int pageSize = model.paging.pageSize == null ? 5 : (model.paging.pageSize > 5 ? 5 : model.paging.pageSize.Value);
                facetFilter = await GetGroupOfFacetFilterItem(model.paging.code, sql, parameters, page, pageSize);
                if (facetFilter?.rs != null && facetFilter.rs.Count > 0)
                {
                    facetFilter.forPaging = true;
                    facetFilters.Add(facetFilter);
                }
            }

            return facetFilters;
        }

        private async Task<SearchResponse> InitSearchingAsync(int page, int pageSize, SearchBody model)
        {
            StringBuilder lastSeq = new StringBuilder();
            var last_seq_parameters = new DynamicParameters();
            lastSeq.AppendLine($"SELECT MAX(id) FROM o_item WHERE tenant_code = ANY(@_tenantCodes)");

            var tenantCodes = SearchQueryHelper.GetTenantFromFilterBy(model.filterBy);

            if (tenantCodes.Any())
            {
                last_seq_parameters.Add("_tenantCodes", tenantCodes);
            }
            else
            {
                last_seq_parameters.Add("_tenantCodes", _tenantCodes);
            }

            LoggerHelper.LogQuery(_enableQueryLog, _logger, "Query get MAX id o_item: {NewLine}{Sql}", Environment.NewLine, lastSeq);
            int? lastId = await _unitOfWork.Repository.QueryFirstAsync<int?>(lastSeq.ToString(), last_seq_parameters);
            StringBuilder query = new StringBuilder();
            var (builtQuery, parameters) = _intermediateSearchLogic.InitBuildQuerySearch(lastId ?? 0, model);
            query.AppendLine(builtQuery);
            var (builtQueryFilter, parameters2) = SearchQueryHelper.BuildQueryFilter(model);
            query.AppendLine(builtQueryFilter);
            parameters.AddDynamicParams(parameters2);

            StringBuilder countQuery = new StringBuilder();
            countQuery.AppendLine(query.ToString().Replace(ConstQuery.SelectDocumentViewQuery, "SELECT COUNT(*)"));
            query.AppendLine(SearchQueryHelper.BuildQueryOrderAndOffset(page, pageSize, model));

            LoggerHelper.LogQuery(_enableQueryLog, _logger, "Query count INIT search: {NewLine}{Sql}", Environment.NewLine, countQuery);
            int count = await _unitOfWork.Repository.QueryFirstAsync<int>(countQuery.ToString(), parameters);
            LoggerHelper.LogQuery(_enableQueryLog, _logger, "Query INIT search: {NewLine}{Sql}", Environment.NewLine, countQuery);
            List<Result> items = await _unitOfWork.Repository.QueryListAsync<Result>(query.ToString(), parameters);
            var totalRecords = count;
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            SearchResponse searchResult = new SearchResponse();

            searchResult.currentPage = page;
            searchResult.totalPages = totalPages;
            searchResult.pageSize = pageSize;
            searchResult.totalRecords = totalRecords;
            searchResult.results = items;

            return searchResult;
        }

        #region QuickSearch

        private async Task<SearchResponse> QuickSearchingAsync(int page, int pageSize, SearchBody model)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            StringBuilder query = new StringBuilder();

            var (builtQuery, parameters) = _intermediateSearchLogic.QuickBuildQuerySearch(model);
            query.AppendLine(builtQuery);
            var (builtQueryFilter, parameters2) = SearchQueryHelper.BuildQueryFilter(model);
            query.AppendLine(builtQueryFilter);
            parameters.AddDynamicParams(parameters2);

            StringBuilder countQuery = new StringBuilder();
            countQuery.AppendLine(query.ToString().Replace(ConstQuery.SelectDocumentViewQuery, "SELECT COUNT(*)"));

            query.AppendLine(SearchQueryHelper.BuildQueryOrderAndOffset(page, pageSize, model));

            LoggerHelper.LogQuery(_enableQueryLog, _logger, "Query count QUICK search: {NewLine}{Sql}", Environment.NewLine, countQuery);
            int count = await _unitOfWork.Repository.QueryFirstAsync<int>(countQuery.ToString(), parameters);
            LoggerHelper.LogQuery(_enableQueryLog, _logger, "Query QUICK search: {NewLine}{Sql}", Environment.NewLine, query);
            List<Result> items = await _unitOfWork.Repository.QueryListAsync<Result>(query.ToString(), parameters);
            var totalRecords = count;
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            SearchResponse searchResult = new SearchResponse();

            searchResult.currentPage = page;
            searchResult.totalPages = totalPages;
            searchResult.pageSize = pageSize;
            searchResult.totalRecords = totalRecords;
            searchResult.results = items;

            return searchResult;
        }

        private async Task<List<FacetFilterResponse>> InitSearchingFacetFilterAsync(FacetFilterRequest model)
        {
            StringBuilder lastSeq = new StringBuilder();
            var last_seq_parameters = new DynamicParameters();
            lastSeq.AppendLine("SELECT MAX(id) from o_item WHERE tenant_code = ANY(@_tenantCodes)");

            var tenantCodes = SearchQueryHelper.GetTenantFromFilterBy(model.searchRequest.filterBy);

            if (tenantCodes.Any())
            {
                last_seq_parameters.Add("_tenantCodes", tenantCodes);
            }
            else
            {
                last_seq_parameters.Add("_tenantCodes", _tenantCodes);
            }

            int? lastId = await _unitOfWork.Repository.QueryFirstAsync<int?>(lastSeq.ToString(), last_seq_parameters);

            StringBuilder sql = new StringBuilder();

            var (builtQuery, parameters) = _intermediateSearchLogic.InitBuildQuerySearch(lastId ?? 0, model.searchRequest);
            sql.AppendLine(builtQuery);

            StringBuilder query = new StringBuilder();
            query.AppendLine(sql.ToString());
            var (builtQueryFilter, parameters2) = SearchQueryHelper.BuildQueryFilter(model.searchRequest);
            query.AppendLine(builtQueryFilter);
            parameters.AddDynamicParams(parameters2);

            List<FacetFilterResponse> facetFilters = await QueryFacetFilterAsync(query, model, parameters);
            return facetFilters;
        }

        private async Task<List<FacetFilterResponse>> QuickFacetFilterAsync(FacetFilterRequest model)
        {
            StringBuilder query = new StringBuilder();
            var (builtQuery, parameters) = _intermediateSearchLogic.QuickBuildQuerySearch(model.searchRequest);
            query.AppendLine(builtQuery);
            var (builtQueryFilter, parameters2) = SearchQueryHelper.BuildQueryFilter(model.searchRequest);
            query.AppendLine(builtQueryFilter);
            parameters.AddDynamicParams(parameters2);

            List<FacetFilterResponse> facetFilters = await QueryFacetFilterAsync(query, model, parameters);
            return facetFilters;
        }
        #endregion QuickSearch

        #region BasicSearch
        private async Task<List<FacetFilterResponse>> BasicFacetFilterAsync(FacetFilterRequest model)
        {
            StringBuilder query = new StringBuilder();
            var (builtQuery, parameters) = _intermediateSearchLogic.BasicBuildQuerySearch(model.searchRequest);
            query.AppendLine(builtQuery);
            var (builtQueryFilter, parameters2) = SearchQueryHelper.BuildQueryFilter(model.searchRequest);
            query.AppendLine(builtQueryFilter);
            parameters.AddDynamicParams(parameters2);

            List<FacetFilterResponse> facetFilters = await QueryFacetFilterAsync(query, model, parameters);
            return facetFilters;
        }


        private async Task<SearchResponse> BasicSearchingAsync(int page, int pageSize, SearchBody model)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            StringBuilder query = new StringBuilder();
            var (builtQuery, parameters) = _intermediateSearchLogic.BasicBuildQuerySearch(model);
            query.AppendLine(builtQuery);
            var (builtQueryFilter, parameters2) = SearchQueryHelper.BuildQueryFilter(model);
            query.AppendLine(builtQueryFilter);
            parameters.AddDynamicParams(parameters2);

            StringBuilder countQuery = new StringBuilder();
            countQuery.AppendLine(query.ToString().Replace(ConstQuery.SelectDocumentViewQuery, "SELECT COUNT(*)"));

            query.AppendLine(SearchQueryHelper.BuildQueryOrderAndOffset(page, pageSize, model));

            LoggerHelper.LogQuery(_enableQueryLog, _logger, "Query count BASIC search: {NewLine}{Sql}", Environment.NewLine, countQuery);
            int count = await _unitOfWork.Repository.QueryFirstAsync<int>(countQuery.ToString(), parameters);
            LoggerHelper.LogQuery(_enableQueryLog, _logger, "Query BASIC search: {NewLine}{Sql}", Environment.NewLine, query);
            List<Result> items = await _unitOfWork.Repository.QueryListAsync<Result>(query.ToString(), parameters);
            var totalRecords = count;
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            SearchResponse searchResult = new SearchResponse();

            searchResult.currentPage = page;
            searchResult.totalPages = totalPages;
            searchResult.pageSize = pageSize;
            searchResult.totalRecords = totalRecords;
            searchResult.results = items;

            return searchResult;
        }
        #endregion BasicSearch

        #region AdvanceSearch

        private async Task<SearchResponse> AdvanceSearchingAsync(int page, int pageSize, SearchBody model)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            StringBuilder query = new StringBuilder();
            var (builtQuery, parameters) = _intermediateSearchLogic.AdvanceBuildQuerySearch(model);
            query.AppendLine(builtQuery);
            var (builtQueryFilter, parameters2) = SearchQueryHelper.BuildQueryFilter(model);
            query.AppendLine(builtQueryFilter);
            parameters.AddDynamicParams(parameters2);

            StringBuilder countQuery = new StringBuilder();
            countQuery.AppendLine(query.ToString().Replace(ConstQuery.SelectDocumentViewQuery, "SELECT COUNT(*)"));

            query.AppendLine(SearchQueryHelper.BuildQueryOrderAndOffset(page, pageSize, model));

            LoggerHelper.LogQuery(_enableQueryLog, _logger, "Query count ADVANCE search: {NewLine}{Sql}", Environment.NewLine, countQuery);
            int count = await _unitOfWork.Repository.QueryFirstAsync<int>(countQuery.ToString(), parameters);
            LoggerHelper.LogQuery(_enableQueryLog, _logger, "Query ADVANCE search: {NewLine}{Sql}", Environment.NewLine, query);
            List<Result> items = await _unitOfWork.Repository.QueryListAsync<Result>(query.ToString(), parameters);
            var totalRecords = count;
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            SearchResponse searchResult = new SearchResponse();

            searchResult.currentPage = page;
            searchResult.totalPages = totalPages;
            searchResult.pageSize = pageSize;
            searchResult.totalRecords = totalRecords;
            searchResult.results = items;

            return searchResult;
        }

        private async Task<List<FacetFilterResponse>> AdvanceFacetFilterAsync(FacetFilterRequest model)
        {
            StringBuilder query = new StringBuilder();
            var (builtQuery, parameters) = _intermediateSearchLogic.AdvanceBuildQuerySearch(model.searchRequest);
            query.AppendLine(builtQuery);
            var (builtQueryFilter, parameters2) = SearchQueryHelper.BuildQueryFilter(model.searchRequest);
            query.AppendLine(builtQueryFilter);
            parameters.AddDynamicParams(parameters2);

            return await QueryFacetFilterAsync(query, model, parameters);
        }
        #endregion AdvanceSearch

        public async Task<SearchResponse> SearchingAsync(string type, int page, int pageSize, SearchBody searchRequest)
        {
            string searchRequestJson = JsonSerializer.Serialize(searchRequest, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            string hashKey = HashHelper.ComputeMD5Hash(searchRequestJson);
            string cacheKey = $"Search_{type}_{page}_{pageSize}_{hashKey}";

            bool enableCache = _appConfigHelper.GetEnableCacheResultSearch();
            int cacheInMinutes = _appConfigHelper.GetCacheResultSearchInMinute();
            int slidingCache = _appConfigHelper.GetSlidingCacheInMinute();

            try
            {
                if (!enableCache || !_memoryCache.TryGetValue(cacheKey, out SearchResponse? cachedResult))
                {
                    cachedResult = type.ToLower() switch
                    {
                        "init" => await InitSearchingAsync(page, pageSize, searchRequest),
                        "quick" => await QuickSearchingAsync(page, pageSize, searchRequest),
                        "basic" => await BasicSearchingAsync(page, pageSize, searchRequest),
                        "advance" => await AdvanceSearchingAsync(page, pageSize, searchRequest),
                        _ => new SearchResponse(),
                    };

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
            catch (Exception ex)
            {
                LoggerHelper.LogError(_logger, ex, ex.Message);
                return new();
            }
        }

        public async Task<List<FacetFilterResponse>> FacetFilterAsync(string type, FacetFilterRequest facetFilterRequest)
        {
            string searchRequestJson = JsonSerializer.Serialize(facetFilterRequest, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            string hashKey = HashHelper.ComputeMD5Hash(searchRequestJson);
            string cacheKey = $"Search_{type}_{hashKey}";

            bool enableCache = _appConfigHelper.GetEnableCacheResultSearch();
            int cacheInMinutes = _appConfigHelper.GetCacheResultSearchInMinute();
            int slidingCache = _appConfigHelper.GetSlidingCacheInMinute();

            try
            {
                if (!enableCache || !_memoryCache.TryGetValue(cacheKey, out List<FacetFilterResponse>? cachedResult))
                {
                    cachedResult = type.ToLower() switch
                    {
                        "init" => await InitSearchingFacetFilterAsync(facetFilterRequest),
                        "quick" => await QuickFacetFilterAsync(facetFilterRequest),
                        "basic" => await BasicFacetFilterAsync(facetFilterRequest),
                        "advance" => await AdvanceFacetFilterAsync(facetFilterRequest),
                        _ => throw new Exception($"Invalid type!")
                    };

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
            catch (Exception ex)
            {
                LoggerHelper.LogError(_logger, ex, ex.Message);
                return new();
            }
        }
    }
}