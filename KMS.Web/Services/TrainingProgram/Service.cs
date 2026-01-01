using KMS.Shared.DTOs.TrainingProgram;
using KMS.Shared.DTOs.Search;
using KMS.Shared.Helpers;
using KMS.Web.Helpers;

namespace KMS.Web.Services.TrainingProgram
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

        public async Task<TrainingPrograms> GetDetailAsync(string id)
        {
            var baseUrl = _appConfigHelper.GetApiApp();
            var url = baseUrl + $"TrainingProgram/get-list-section-by-system-id?id={id}";
            var response = await _apiHelper.GetApiResponseAsync<TrainingPrograms>(url);
            if (response.Success)
            {
                return response.Data ?? new();
            }

            return new();
        }

        public async Task<TrainingSections> GetDetailSectionAsync(string id)
        {
            var baseUrl = _appConfigHelper.GetApiApp();
            var url = baseUrl + $"TrainingProgram/get-detail-section-by-id?id={id}";
            var response = await _apiHelper.GetApiResponseAsync<TrainingSections>(url);
            if (response.Success)
            {
                return response.Data ?? new();
            }

            return new();
        }
    }
}
