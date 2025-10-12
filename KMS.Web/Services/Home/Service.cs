using KMS.Shared.DTOs.Document;
using KMS.Shared.Helpers;
using KMS.Web.Helpers;
using KMS.Web.ViewModels.Shared.Components.Home;

namespace KMS.Web.Services.Home
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

        public async Task<List<DocumentNew>> GetTopDocumentsNewAsync()
        {
            var apiApp = _appConfigHelper.GetApiApp();
            var url = apiApp + "Document/get-top-documet-new";
            var response = await _apiHelper.GetApiResponseAsync<List<DocumentNew>>(url);

            if (response.Success)
            {
                return response.Data ?? new();
            }

            return new();
        }

        public async Task<List<CollectionDto>> GetTopBibCollection()
        {
            var apiApp = _appConfigHelper.GetApiApp();
            var url = apiApp + "Document/get-top-bib-collection";
            var response = await _apiHelper.GetApiResponseAsync<List<CollectionDto>>(url);

            if (response.Success)
            {
                return response.Data ?? new();
            }

            return new();
        }
    }
}
