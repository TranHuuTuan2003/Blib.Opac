using System.Collections.Concurrent;

using Microsoft.AspNetCore.Html;

namespace KMS.Web.Helpers
{
    public static class SvgHelper
    {
        private static readonly ConcurrentDictionary<string, string> _svgCache = new();

        public static IHtmlContent GetSvgIcon(string iconName, bool isFilled = false)
        {
            var key = $"{iconName}_{isFilled}";

            // Dùng GetOrAdd để tránh race condition
            var svgContent = _svgCache.GetOrAdd(key, _ =>
            {
                var filePath = Path.Combine("wwwroot", "img", "icons", $"{iconName}{(isFilled ? "_f" : "_o")}.svg");

                if (!File.Exists(filePath))
                    return $"<!-- SVG {iconName} not found -->";

                return File.ReadAllText(filePath);
            });

            return new HtmlString(svgContent);
        }
    }
}
