using KMS.Shared.Helpers;
using KMS.Web.Core;
using KMS.Web.Helpers;

namespace KMS.Web.Services.Menu
{
    public class Service : IService
    {
        private readonly ApiHelper _apiHelper;
        private readonly AppConfigHelper _appConfigHelper;
        private readonly ILogger<Service> _logger;
        private readonly JsonConfigCacheService<List<Shared.DTOs.Menu.Menu>> _jsonConfigCache;

        public Service(ApiHelper apiHelper, AppConfigHelper appConfigHelper, ILogger<Service> logger, JsonConfigCacheService<List<Shared.DTOs.Menu.Menu>> jsonConfigCache)
        {
            _apiHelper = apiHelper;
            _appConfigHelper = appConfigHelper;
            _logger = logger;
            _jsonConfigCache = jsonConfigCache;
        }

        public async Task<List<Shared.DTOs.Menu.Menu>> GetMenuAsync()
        {
            var menus = _jsonConfigCache.GetConfig();
            if (menus == null || !menus.Any())
            {
                var base_url = _appConfigHelper.GetApiApp();
                var url = base_url + "PMenu/get-menus";
                try
                {
                    var response = await _apiHelper.GetApiResponseAsync<List<Shared.DTOs.Menu.Menu>>(url);
                    if (response.Data != null)
                    {
                        _jsonConfigCache.SaveConfig(response.Data);
                        return response.Data;
                    }
                    return new();
                }
                catch (Exception ex)
                {
                    LoggerHelper.LogError(_logger, ex, ex.Message);
                    return new();
                }
            }

            return menus;
        }
    }
}