
using KMS.Shared.DTOs.Search;

namespace KMS.Web.Services.PLog
{
    public interface IService
    {
        Task CreateLogDailyAsync(HttpContext context);
        Task CreateLogDetailAsync(HttpContext context, string env, string actionType, int eventType, string content);
        Task CreateSearchLogDetailAsync(HttpContext context, SearchBody model, string eventType);
        //Task SyncLogToCoreAsync();
    }
}