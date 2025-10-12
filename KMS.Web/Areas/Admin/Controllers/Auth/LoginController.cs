using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using KMS.Web.Helpers;
using KMS.Web.Areas.Admin.Common;
using KMS.Web.Areas.Admin.Models.Auth;
using UC.Core.Common;
using UC.Core.Models;

namespace KMS.Web.Areas.Admin.Controllers.Auth
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppConfigHelper _appConfigHelper;

        public LoginController(ILogger<LoginController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, AppConfigHelper appConfigHelper)
        {
            _logger = logger;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _appConfigHelper = appConfigHelper;
        }

        [Route("dang-nhap")]
        public async Task<IActionResult> Index()
        {
            LoginModel model = new LoginModel();

            string address = _appConfigHelper.GetApiCore();
            string appCode = _appConfigHelper.GetAppCode();
            string baseUrlFile = _appConfigHelper.GetBaseUrlFile();

            string client_site = _httpContextAccessor.HttpContext.Request.Cookies["client_site"];
            string lang = _httpContextAccessor.HttpContext.Request.Cookies["lang"];

            if (string.IsNullOrEmpty(client_site))
            {
                client_site = _appConfigHelper.GetTenantCode();
            }

            if (string.IsNullOrEmpty(lang))
            {
                lang = _appConfigHelper.GetLang();
            }

            try
            {
                ClientRequestInfo clientRequestInfo = new ClientRequestInfo(address);
                clientRequestInfo.Cookies = new Dictionary<string, string>()
                {
                    { UC.Core.Common.CookieKeys.ClientSite, Request.Cookies[UC.Core.Common.CookieKeys.ClientSite] }
                };
                clientRequestInfo.ValueHeaderUcSite = client_site;
                HttpClientBuilder httpClientBuilder = new HttpClientBuilder(clientRequestInfo);
                ClientResponseInfo clientResponseInfo = httpClientBuilder.GetAsync($"Ums_App/get-by-app-code/{appCode}").GetAwaiter().GetResult();
                if (clientResponseInfo.IsStatusCode)
                {
                    ClientResponseResult<LoginModel> responseResult = JsonConvert.DeserializeObject<ClientResponseResult<LoginModel>>(clientResponseInfo.Content);
                    if (responseResult != null && responseResult.Data != null)
                    {
                        model.name = responseResult.Data.name;
                        model.description = responseResult.Data.description;
                        model.logo = baseUrlFile + responseResult.Data.logo;
                        model.logo_login = baseUrlFile + responseResult.Data.logo_login;
                        model.login_background = baseUrlFile + responseResult.Data.login_background;
                        model.sidebar_background = baseUrlFile + responseResult.Data.sidebar_background;
                    }
                }
            }
            catch
            {

            }

            Response.Cookies.Append(
                UC.Core.Common.CookieKeys.ClientSite,
                client_site,
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            Response.Cookies.Append(
                "lang",
                lang,
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            return View(AdminViewPaths.auth + "Login/Index.cshtml", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}