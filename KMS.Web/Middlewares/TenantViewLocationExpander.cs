using Microsoft.AspNetCore.Mvc.Razor;

namespace KMS.Web.Middlewares
{
    public class TenantViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            var tenant = context.ActionContext.HttpContext.Items["Tenant"]?.ToString();
            context.Values["tenant"] = tenant ?? "Default";
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            string tenant = context.Values["tenant"];
            if (!string.IsNullOrEmpty(tenant))
            {
                // Ưu tiên view theo tenant trước
                var tenantViewLocations = new[]
                {
                $"/Views/Tenants/{tenant}/{{1}}/{{0}}.cshtml",
                $"/Views/Tenants/{tenant}/Shared/{{0}}.cshtml"
            };
                viewLocations = tenantViewLocations.Concat(viewLocations);
            }

            return viewLocations;
        }
    }
}