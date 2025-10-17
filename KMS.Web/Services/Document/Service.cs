using KMS.Shared.DTOs.Document;
using KMS.Shared.Helpers;
using KMS.Web.Helpers;

namespace KMS.Web.Services.Document
{
    public class Service : IService
    {
        private readonly ILogger<Service> _logger;
        private readonly ApiHelper _apiHelper;
        private readonly AppConfigHelper _appConfigHelper;

        public Service(ILogger<Service> logger, ApiHelper apiHelper, AppConfigHelper appConfigHelper)
        {
            _logger = logger;
            _apiHelper = apiHelper;
            _appConfigHelper = appConfigHelper;
        }

        public async Task<Details?> GetDocumentDetailAsync(string slug)
        {
            var baseUrl = _appConfigHelper.GetApiApp();
            var url = baseUrl + $"Document/get-detail?slug={Uri.EscapeDataString(slug)}";
            var response = await _apiHelper.GetApiResponseAsync<Details>(url);

            return response.Data;
        }

        public async Task<List<Result>> GetRelatedDocumentsAsync(string slug)
        {
            var apiApp = _appConfigHelper.GetApiApp();
            var url = apiApp + "Document/related-documents?slug=" + slug + "&limit=" + 3;
            var response = await _apiHelper.GetApiResponseAsync<List<Result>>(url);

            if (response.Success)
            {
                return response.Data ?? new();
            }

            LoggerHelper.LogError(_logger, new Exception(), "Error when getting related documents of slug: {slug}, error: {error}", slug, response.Message ?? "Unknown error!");
            return new();
        }

        public async Task<List<Result>> GetTop6BibHot()
        {
            var apiApp = _appConfigHelper.GetApiApp();
            var url = apiApp + "Document/get-top-documet-hot";
            var response = await _apiHelper.GetApiResponseAsync<List<Result>>(url);

            if (response.Success)
            {
                return response.Data ?? new();
            }

            return new();
        }

        
    }
}
