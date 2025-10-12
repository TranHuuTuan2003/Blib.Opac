using KMS.Shared.DTOs.Search;
using KMS.Web.Helpers;
using Newtonsoft.Json;

using Uc.Services.Dtos.Lms;
using Uc.Services.Services.Lms;

using static KMS.Web.Common.Enums;

namespace KMS.Web.Services.PLog
{
    public class SearchLogContent
    {
        public string key { get; set; } = string.Empty;
        public string value { get; set; } = string.Empty;

        public SearchLogContent(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }

    public class Service : IService
    {
        private readonly ILogger<Service> _logger;
        private readonly IConfiguration _configuration;
        private readonly ApiHelper _apiHelper;
        private readonly AppConfigHelper _appConfigHelper;

        public Service(
            ILogger<Service> logger,
            IConfiguration configuration,
            ApiHelper apiHelper,
            AppConfigHelper appConfigHelper
            )
        {
            _logger = logger;
            _configuration = configuration;
            _apiHelper = apiHelper;
            _appConfigHelper = appConfigHelper;
        }

        public async Task CreateLogDailyAsync(HttpContext context)
        {
            try
            {
                await LogService.LogAccessDailyAsync(context, _configuration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating daily log");
            }
        }

        public async Task CreateLogDetailAsync(HttpContext context, string env, string actionType, int eventType, string content)
        {
            try
            {
                await LogService.LogAccessDetailAsync(context, new GeneralAccessLog(env, actionType, eventType, content, null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating detail log");
            }
        }

        private List<SearchLogContent> GetKeyValuePairs(SearchBody model)
        {
            List<SearchLogContent> searchLogContents = new List<SearchLogContent>();
            foreach (var item in model.searchBy)
            {
                searchLogContents.Add(new SearchLogContent(item[0], item[1]));
            }
            return searchLogContents;
        }

        public async Task CreateSearchLogDetailAsync(HttpContext context, SearchBody model, string eventType)
        {
            if (eventType.ToLower() == "init") return;

            try
            {
                int intEventType = eventType switch
                {
                    "quick" => (int)StatisticType.QUICK_SEARCH,
                    "basic" => (int)StatisticType.BASIC_SEARCH,
                    "advance" => (int)StatisticType.ADVANCE_SEARCH,
                    _ => (int)StatisticType.UNKNOWN_SEARCH,
                };
                if (intEventType == 20)
                {
                    var searchContent = new
                    {
                        option = model?.searchBy?[0][1],
                        keyword = model?.searchBy?[1][1],
                    };
                    await LogService.LogAccessDetailAsync(context, new GeneralAccessLog("publish", "s", intEventType, JsonConvert.SerializeObject(searchContent), null));
                }
                else if (intEventType == 21 || intEventType == 22)
                {
                    var searchLogContents = GetKeyValuePairs(model);
                    await LogService.LogAccessDetailAsync(context, new GeneralAccessLog("publish", "s", intEventType, JsonConvert.SerializeObject(searchLogContents), null));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating search detail log");
            }
        }

        //public async Task SyncLogToCoreAsync()
        //{
        //    try
        //    {
        //        List<access_daily> accessDaily = await LogService.ReadAccessDailyFilesAsync();
        //        List<access_detail> accessDetail = await LogService.ReadAccessDetailFilesAsync();

        //        var coreUrl = _appConfigHelper.GetApiCore();
        //        var appCode = _appConfigHelper.GetAppCode();

        //        var response_1 = await _apiHelper.PostApiResponseAsync<object>(
        //            coreUrl + "Lms_Access_Log/insert-multi-access-daily?app_code=" + appCode, accessDaily);
        //        if (response_1.Success)
        //        {
        //            LogService.DeleteOldFiles("AccessLog/AccessDaily", "access_daily_");
        //        }

        //        var response_2 = await _apiHelper.PostApiResponseAsync<object>(
        //            coreUrl + "Lms_Tracking/insert-multi-access-detail?app_code=" + appCode, accessDetail);
        //        if (response_2.Success)
        //        {
        //            LogService.DeleteOldFiles("AccessLog/AccessDetail", "access_detail_");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Failed to update digital file view: {ex.Message}");
        //    }
        //}
    }
}