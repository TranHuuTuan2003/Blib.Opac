using KMS.Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KMS.Api.Filters
{
    public class OnlyAppAuthorizationAttribute : ActionFilterAttribute
    {
        private readonly string UC_APP_NAME = "UcApp";
        private readonly string API_KEY_SECRET_NAME = "X-UC-SECRET";
        private readonly string _appCode = "kms";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var appConfigHelper = context.HttpContext.RequestServices.GetRequiredService<AppConfigHelper>();
            var secretApiKey = appConfigHelper.GetApiKey();
            var headers = context.HttpContext.Request.Headers;

            if (headers.TryGetValue(UC_APP_NAME, out var appCode) && appCode == _appCode)
            {
                if (headers.TryGetValue(API_KEY_SECRET_NAME, out var secretKey) && secretKey == secretApiKey)
                {
                    base.OnActionExecuting(context);
                    return;
                }
            }

            context.Result = new UnauthorizedResult();
        }
    }
}