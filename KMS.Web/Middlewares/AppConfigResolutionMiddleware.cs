using KMS.Web.Common;
using KMS.Web.Helpers;

namespace KMS.Web.Middlewares
{
    public class AppConfigResolutionMiddleware
    {
        private readonly RequestDelegate _next;

        public AppConfigResolutionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var appConfigHelper = context.RequestServices.GetRequiredService<AppConfigHelper>();
            context.Items["Tenant"] = appConfigHelper.GetTenantCode() ?? "default";
            context.Items["EnableESearch"] = appConfigHelper.GetEnabledESearch();
            context.Items["Layout"] = "~/Pages/Shared/_LayoutPublish.cshtml";
            context.Items["BaseUrlFile"] = ConstLocation.value + appConfigHelper.GetBaseUrlFile();

            await _next(context);
        }
    }
}