using KMS.Shared.DTOs.Document;
using KMS.Shared.Helpers;
using KMS.Web.Helpers;

namespace KMS.Web.Services.DigitalFile
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

        public async Task<string> GetFile(string id)
        {
            var apiApp = _appConfigHelper.GetApiApp();
            var url = apiApp + "Document/get-file-pdf?id=" + id;
            var response = await _apiHelper.GetApiResponseAsync<string>(url);

            if (response.Success)
            {
                return response.Data ?? "";
            }

            return "";
        }
    }
}
