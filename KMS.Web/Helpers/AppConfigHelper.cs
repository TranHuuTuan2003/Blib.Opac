using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Caching.Memory;

using KMS.Web.Core;
using KMS.Web.Models.JsonConfig;

namespace KMS.Web.Helpers
{

    public class AppConfigHelper
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _cache;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<JsonConfigCacheService<DocumentInfoLabelByLanguage>> _logger;

        public AppConfigHelper(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMemoryCache cache, IWebHostEnvironment env, ILogger<JsonConfigCacheService<DocumentInfoLabelByLanguage>> logger)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
            _env = env;
            _logger = logger;
        }

        public string GetApiApp()
        {
            return _configuration.GetSection("Services:ApiApp").Value ?? "";
        }

        public string GetApiCore()
        {
            return _configuration.GetSection("Services:ApiCore").Value ?? "";
        }

        public string GetApiBlib()
        {
            return _configuration.GetSection("Services:ApiBlib").Value ?? "";
        }

        public string GetApiLog()
        {
            return _configuration.GetSection("Services:ApiLog").Value ?? "";
        }

        public string GetApiESearch()
        {
            return _configuration.GetSection("Services:ApiESearch").Value ?? "";
        }

        public string GetLang()
        {
            return _configuration.GetSection("AppConfig:Lang").Value ?? "";
        }

        public string GetAppCode()
        {
            return _configuration.GetSection("AppConfig:AppCode").Value ?? "";
        }

        public string GetAuthenticationCode()
        {
            return _configuration.GetSection("AppConfig:AuthenticationCode").Value ?? "";
        }

        public string GetBaseUrlFile()
        {
            return _configuration.GetSection("BaseUrlFile").Value ?? "";
        }

        public string GetBaseUrlGateway()
        {
            return _configuration.GetSection("BaseUrlGateway").Value ?? "";
        }

        public string GetCmsLink()
        {
            return _configuration.GetSection("CmsLink").Value ?? "";
        }

        public string GetHashKey()
        {
            return _configuration.GetSection("AppConfig:HashKey").Value ?? "";
        }

        public string GetTenantCode()
        {
            return _configuration.GetSection("AppConfig:ClientSite").Value ?? "";
        }

        public string GetSecretKey()
        {
            return _configuration.GetSection("DSecretKey").Value ?? "";
        }

        public string GetApiKey()
        {
            return _configuration.GetSection("ApiKey").Value ?? "";
        }

        public bool GetEnabledDisplay9PrefixOfMarc21()
        {
            return bool.Parse(_configuration.GetSection("AppConfig:Display9PrefixOfMarc21").Value ?? "false");
        }

        public bool GetEnabledESearch()
        {
            return bool.Parse(_configuration.GetSection("AppConfig:EnableESearch").Value ?? "false");
        }

        public int GetSessionTimeout()
        {
            return int.Parse(_configuration.GetSection("SessionTimeout").Value ?? "0");
        }

        public IdentityServerAuthentication GetIdentityServerAuthentication()
        {
            return _configuration.GetSection("IdentityServerAuthentication").Get<IdentityServerAuthentication>() ?? new();
        }

        public bool GetEnableDownloadImageSource()
        {
            return bool.Parse(_configuration.GetSection("EnableDownloadImageSources").Value ?? "false");
        }

        public bool GetEnableSTT()
        {
            return bool.Parse(_configuration.GetSection("AppConfig:EnableSTT").Value ?? "false");
        }

        public string GetSeparatorSemicolon()
        {
            return _configuration.GetSection("AppConfig:SeperatorSemicolon").Value ?? "w1w";
        }

        public string GetSeparatorSymbol()
        {
            return _configuration.GetSection("AppConfig:SeparatorSymbol").Value ?? ";";
        }

        public string GetCurrentLanguage()
        {
            return _httpContextAccessor?.HttpContext?.Request.Cookies["lang"] ?? "vi";
        }

        public List<DocumentInfoLabel> GetBookInfoKeyAndLabels()
        {
            string currentLanguage = GetCurrentLanguage();
            var service = DocumentLabelService;
            return currentLanguage == "vi" ? service.GetConfig().labels_vi : service.GetConfig().labels_en;
        }

        public List<string> GetBookInfoDisplayOrder()
        {
            var service = DocumentLabelService;
            string currentLanguage = GetCurrentLanguage();
            return currentLanguage == "vi"
            ? service.GetConfig().labels_vi.ToDictionary(x => x.key, x => x.label, StringComparer.OrdinalIgnoreCase).Keys.ToList()
            : service.GetConfig().labels_en.ToDictionary(x => x.key, x => x.label, StringComparer.OrdinalIgnoreCase).Keys.ToList();
        }

        private JsonConfigCacheService<DocumentInfoLabelByLanguage> DocumentLabelService
        {
            get
            {
                var tenantCode = _httpContextAccessor.HttpContext?.Items["Tenant"]?.ToString() ?? "default";
                string path = Path.Combine(_env.WebRootPath, "configs", tenantCode, "document_info_labels", "document_info_labels.json");
                return new JsonConfigCacheService<DocumentInfoLabelByLanguage>(
                    "DocumentInfoLabelByLanguage",
                    path,
                    _cache,
                    _logger
                );
            }
        }

        public string GenerateHMACHash(string key, string value)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(value));
                var base64 = Convert.ToBase64String(hash);
                return base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
            }
        }
    }
}