using KMS.Shared.DTOs.Api;
using KMS.Shared.DTOs.FacetFilter;
using KMS.Shared.DTOs.Search;
using KMS.Web.Helpers;
using KMS.Web.ViewModels.Shared.Components.SearchPage;
using KMS.Web.ViewModels.Shared.Pages.Chatbot;

namespace KMS.Web.Services.Chatbot
{
    public class Service : IService
    {
        private readonly ApiHelper _apiHelper;
        private readonly AppConfigHelper _appConfigHelper;

        public Service(AppConfigHelper appConfigHelper, ApiHelper apiHelper)
        {
            _appConfigHelper = appConfigHelper;
            _apiHelper = apiHelper;
        }

        public async Task<ChatbotViewModel> SearchDocBotAsync(SearchRequest searchRequest, bool hasFacetFilter)
        {
            if (searchRequest.request == null) return new();

            var apiApp = _appConfigHelper.GetApiApp();
            var type = searchRequest.type ?? "quick";
            var sortType = string.IsNullOrEmpty(searchRequest?.request?.sortBy?[0][1]) ? "desc" : searchRequest.request.sortBy[0][1];
            var page = searchRequest?.page <= 1 ? 1 : searchRequest?.page;
            var pageSize = searchRequest?.pageSize > 10 ? 10 : searchRequest?.pageSize;
            var url = apiApp + $"Search/{type}/{page}/{pageSize}";
            var facetUrl = apiApp + $"Search/FacetFilterAsync/{type}";
            var facetFilterRequest = new FacetFilterRequest();
            facetFilterRequest.searchRequest = searchRequest.request;
            facetFilterRequest.codes = (facetFilterRequest.codes ?? Array.Empty<string>()).Append("bt").ToArray();
            facetFilterRequest.paging = new FacetFilterPagingRequest { code = "bt", page = 1, pageSize = 3 };
            facetFilterRequest.pagings = new();

            var responseTask = _apiHelper.PostApiResponseAsync<SearchResultViewModel>(url, searchRequest!.request);

            var tasks = new List<Task> { responseTask };

            Task<ApiResponse<List<FacetFilterResponse>>>? facetFilterTask = null;

            if (hasFacetFilter)
            {
                facetFilterTask = _apiHelper.PostApiResponseAsync<List<FacetFilterResponse>>(facetUrl, facetFilterRequest);
                tasks.Add(facetFilterTask);
            }

            // Chạy tất cả task song song
            await Task.WhenAll(tasks);

            var model = new ChatbotViewModel();
            if (responseTask.Result.Data != null)
            {
                responseTask.Result.Data.sortType = sortType;
                model.searchResult = responseTask.Result.Data;
                model.facetFilters = facetFilterTask?.Result.Data ?? new();
                model.keyword = searchRequest?.request?.searchBy?[1]?[1] ?? string.Empty;
                model.hasFacetFilter = hasFacetFilter;
                model.searchType = type;
                return model;
            }

            return new();
        }
    }
}