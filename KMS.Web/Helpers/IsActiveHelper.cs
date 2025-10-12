using Microsoft.AspNetCore.Mvc.Rendering;

namespace KMS.Web.Helpers
{
    public static class IsActiveHelper
    {
        public static bool IsActive(this IHtmlHelper htmlHelper, string controller, string action, string route = null)
        {
            var routeData = htmlHelper.ViewContext.RouteData;
            var currentController = routeData.Values["controller"]?.ToString();
            var currentAction = routeData.Values["action"]?.ToString();
            var requestPath = htmlHelper.ViewContext.HttpContext.Request.Path.ToString().Trim('/').ToLower();

            // Normalize các đầu vào
            controller = (controller ?? string.Empty).ToLowerInvariant();
            action = (action ?? string.Empty).ToLowerInvariant();
            route = (route ?? string.Empty).Trim('/').ToLowerInvariant();

            // Nếu là Trang chủ
            if (controller == "home" && action == "index")
            {
                // Nếu route là "trang-chu" hoặc rỗng, và URL đang là / hoặc /trang-chu
                if (string.IsNullOrEmpty(requestPath) || requestPath == "trang-chu")
                {
                    return true;
                }
            }

            // So khớp controller + action
            var isMatch = controller == currentController?.ToLower() && action == currentAction?.ToLower();

            // Nếu có route trong config, thì so sánh path hiện tại với route
            if (!string.IsNullOrEmpty(route))
            {
                isMatch = isMatch && requestPath == route;
            }

            return isMatch;
        }
    }
}