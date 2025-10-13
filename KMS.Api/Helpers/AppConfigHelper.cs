using UC.Core.Models.Ums;

namespace KMS.Api.Helpers
{
    public class AppConfigHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public AppConfigHelper(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public string GetApiApp()
        {
            return _configuration.GetSection("Services:ApiApp").Value ?? "";
        }

        public string GetApiCore()
        {
            return _configuration.GetSection("Services:ApiCore").Value ?? "";
        }

        public string GetApiLog()
        {
            return _configuration.GetSection("Services:ApiLog").Value ?? "";
        }

        public string GetApiBlib()
        {
            return _configuration.GetSection("Services:ApiBlib").Value ?? "";
        }

        public bool GetEnableCacheResultSearch()
        {
            return bool.Parse(_configuration.GetSection("CacheResultSearch:Enable").Value ?? "false");
        }

        public int GetCacheResultSearchInMinute()
        {
            return int.Parse(_configuration.GetSection("CacheResultSearch:TimeInMinutes").Value ?? "0");
        }

        public int GetSlidingCacheInMinute()
        {
            return int.Parse(_configuration.GetSection("CacheResultSearch:Sliding").Value ?? "0");
        }

        public string GetBaseUrlFile()
        {
            return _configuration.GetSection("BaseUrlFile").Value ?? "";
        }

        public string GetHashKey()
        {
            return _configuration.GetSection("AppConfig:HashKey").Value ?? "";
        }

        public string GetSecretKey()
        {
            return _configuration.GetSection("DSecretKey").Value ?? "";
        }

        public string GetApiKey()
        {
            return _configuration.GetSection("ApiKey").Value ?? "";
        }

        public IReadOnlyList<string> GetTenantCodes()
        {
            var multiTenancy = GetMultiTenancy();
            if (multiTenancy)
            {
                var tenant = _httpContextAccessor?.HttpContext?.Request.Headers["UcSite"].ToString()?.ToLower();

                if (!string.IsNullOrEmpty(tenant))
                {
                    return new List<string> { tenant };
                }

                return new List<string> { "default" };
            }
            else
            {
                var section = _configuration.GetSection("AppConfig:ClientSite");

                if (!section.Exists())
                    return Array.Empty<string>();

                if (section.GetChildren().Any())
                {
                    return section
                        .Get<string[]>()
                        ?.Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => s.Trim())
                        .ToArray()
                        ?? Array.Empty<string>();
                }

                var single = section.Value;
                return string.IsNullOrWhiteSpace(single)
                    ? Array.Empty<string>()
                    : [single.Trim()];
            }
        }

        public bool GetMultiTenancy()
        {
            return bool.Parse(_configuration.GetSection("AppConfig:MultiTenancy").Value ?? "false");
        }

        public int GetDueDateRequestInDays()
        {
            return int.Parse(_configuration.GetSection("RegisterBorrow:DueDateRequestInDays").Value ?? "0");
        }

        public List<string> GetAllowedOrigins()
        {
            return _configuration.GetSection("AllowedOrigins").Get<List<string>>() ?? new();
        }

        public bool GetEnableSqlQueryLog()
        {
            return bool.Parse(_configuration.GetSection("AppConfig:EnableSqlQueryLog").Value ?? "false");
        }

        public bool GetEnableMergeRegisterIntoOpac()
        {
            return bool.Parse(_configuration.GetSection("AppConfig:MergeRegisterIntoOpac").Value ?? "false");
        }

        public IdentityServerAuthentication GetIdentityServerAuthentication()
        {
            return _configuration.GetSection("IdentityServerAuthentication").Get<IdentityServerAuthentication>() ?? new();
        }

		public string GetLangFromContext()
		{
			var headers = _httpContextAccessor.HttpContext?.Request.Headers;
			if (headers != null && headers.TryGetValue("Lang", out var langValues))
			{
				var lang = langValues.FirstOrDefault();
				if (!string.IsNullOrEmpty(lang))
					return lang;
			}
			return "vi";
		}
	}
}