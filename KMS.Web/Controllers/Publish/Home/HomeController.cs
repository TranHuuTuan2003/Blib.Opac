using Microsoft.AspNetCore.Mvc;
using KMS.Web.Helpers;
using KMS.Web.Services.PLog;
using SmartBreadcrumbs.Attributes;
using KMS.Web.ViewModels.Shared.Components.Home;

namespace KMS.Web.Controllers.Publish.Home
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppConfigHelper _appConfigHelper;
        private readonly ApiHelper _apiHelper;
        private readonly IService _service;

        public HomeController(ILogger<HomeController> logger, AppConfigHelper appConfigHelper, ApiHelper apiHelper, IService service)
        {
            _logger = logger;
            _appConfigHelper = appConfigHelper;
            _apiHelper = apiHelper;
            _service = service;
        }

        [Route("")]
        [DefaultBreadcrumb("Trang chủ")]
        public IActionResult Index()
        {
            HomeViewModel model = new HomeViewModel();
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}