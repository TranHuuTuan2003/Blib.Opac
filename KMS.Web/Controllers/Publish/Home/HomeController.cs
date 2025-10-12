using KMS.Shared.DTOs.Document;
using KMS.Web.Helpers;
using KMS.Web.Services.Home;
using KMS.Web.ViewModels.Shared.Components.Home;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;

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
        public async Task<IActionResult> Index()
        {
            List<DocumentNew> documents = await _service.GetTopDocumentsNewAsync();
            List<CollectionDto> collections = await _service.GetTopBibCollection();

            HomeViewModel model = new HomeViewModel
            {
                DocumentNews = documents,
                Collections = collections
            };

            return View(model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }


    }
}