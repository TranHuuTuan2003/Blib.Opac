using Microsoft.Extensions.Localization;

namespace KMS.Web.Common.Lang
{
    public class TenantJsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        public TenantJsonStringLocalizerFactory(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return GetClientSiteLang();
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return GetClientSiteLang();
        }

        private IStringLocalizer GetClientSiteLang()
        {
            string client_site = _httpContextAccessor.HttpContext.Request.Cookies["client_site"];
            string lang = _httpContextAccessor.HttpContext.Request.Cookies["lang"];

            if (string.IsNullOrEmpty(client_site))
            {
                client_site = _configuration.GetSection("AppConfig:ClientSite").Value;
            }

            if (string.IsNullOrEmpty(lang))
            {
                lang = _configuration.GetSection("AppConfig:Lang").Value;
            }


            var resourcePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "json");

            return new TenantJsonStringLocalizer(client_site, resourcePath, lang);
        }
    }
}