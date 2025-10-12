using System.Text.Json;

using Microsoft.Extensions.Localization;

namespace KMS.Web.Common.Lang
{
    public class TenantJsonStringLocalizer : IStringLocalizer
    {
        private readonly string _tenant;
        private readonly string _resourcePath;
        private readonly string _culture;

        public TenantJsonStringLocalizer(string tenant, string resourcePath, string culture)
        {
            _tenant = tenant;
            _resourcePath = resourcePath;
            _culture = culture;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value ?? name, value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments] =>
            new(name, string.Format(GetString(name) ?? name, arguments));

        private string? GetString(string name)
        {
            var filePath = Path.Combine(_resourcePath, _tenant, $"{_culture}", "config", "data", "label.json");
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var resources = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (resources != null && resources.TryGetValue(name, out var value))
                {
                    return value;
                }
            }
            return null;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            // Implement if needed for parent culture fallbacks
            return Enumerable.Empty<LocalizedString>();
        }
    }
}