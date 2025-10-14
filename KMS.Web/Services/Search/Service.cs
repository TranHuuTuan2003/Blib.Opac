using KMS.Shared.DTOs.Search;
using KMS.Web.Helpers;
using KMS.Web.ViewModels.Shared.Components.SearchPage;

namespace KMS.Web.Services.Search
{
    public class Service : IService
    {
        private readonly AppConfigHelper _appConfigHelper;
        private readonly ApiHelper _apiHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Service(AppConfigHelper appConfigHelper, ApiHelper apiHelper, IHttpContextAccessor httpContextAccessor)
        {
            _appConfigHelper = appConfigHelper;
            _apiHelper = apiHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<SearchResultViewModel> SearchDocsAsync(SearchRequest searchRequest)
        {
            if (searchRequest?.request == null) return new();

            var apiApp = _appConfigHelper.GetApiApp();
            var type = searchRequest.type ?? "quick";
            var sortType = string.IsNullOrEmpty(searchRequest?.request?.sortBy?[0]?[1]) ? "desc" : searchRequest.request.sortBy[0][1];
            var page = searchRequest?.page <= 1 ? 1 : (searchRequest?.page == null ? 1 : searchRequest?.page);
            var pageSize = searchRequest?.pageSize > 10 ? 10 : (searchRequest?.pageSize == null ? 10 : searchRequest?.pageSize);
            var url = apiApp + $"Search/{type}/{page}/{pageSize}";

            var response = await _apiHelper.PostApiResponseAsync<SearchResultViewModel>(url, searchRequest!.request);
            if (response.Success && response.Data != null)
            {
                response.Data.sortType = sortType;
                return response.Data;
            }

            return new();
        }

        public async Task<SearchResultViewModel> SearchDocsCollectionDdocAsync(SearchRequest searchRequest)
        {
            if (searchRequest?.request == null) return new();

            var apiApp = _appConfigHelper.GetApiApp();
            var type = searchRequest.type ?? "quick";
            var sortType = string.IsNullOrEmpty(searchRequest?.request?.sortBy?[0]?[1]) ? "desc" : searchRequest.request.sortBy[0][1];
            var page = searchRequest?.page <= 1 ? 1 : (searchRequest?.page == null ? 1 : searchRequest?.page);
            var pageSize = searchRequest?.pageSize > 10 ? 10 : (searchRequest?.pageSize == null ? 10 : searchRequest?.pageSize);
            var url = apiApp + $"Collection/GetCollectionItems/{type}/{page}/{pageSize}";

            var response = await _apiHelper.PostApiResponseAsync<SearchResultViewModel>(url, searchRequest!.request);
            if (response.Success && response.Data != null)
            {
                response.Data.sortType = sortType;
                return response.Data;
            }

            return new();
        }

        public async Task<SearchResultViewModel> SearchDocsCollectionPdocAsync(SearchRequest searchRequest)
        {
            if (searchRequest?.request == null) return new();

            var apiApp = _appConfigHelper.GetApiApp();
            var type = searchRequest.type ?? "quick";
            var sortType = string.IsNullOrEmpty(searchRequest?.request?.sortBy?[0]?[1]) ? "desc" : searchRequest.request.sortBy[0][1];
            var page = searchRequest?.page <= 1 ? 1 : (searchRequest?.page == null ? 1 : searchRequest?.page);
            var pageSize = searchRequest?.pageSize > 10 ? 10 : (searchRequest?.pageSize == null ? 10 : searchRequest?.pageSize);
            var url = apiApp + $"Collection/GetCollectionItems/{type}/{page}/{pageSize}";

            var response = await _apiHelper.PostApiResponseAsync<SearchResultViewModel>(url, searchRequest!.request);
            if (response.Success && response.Data != null)
            {
                response.Data.sortType = sortType;
                return response.Data;
            }

            return new();
        }
    }
}