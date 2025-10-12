using Microsoft.AspNetCore.Mvc;
using KMS.Web.Areas.Admin.Common;

namespace KMS.Web.Areas.Admin.Controllers.Ums
{
    [Area("Admin")]
    public class RouteController : Controller
    {
        private readonly ILogger<RouteController> _logger;

        public RouteController(ILogger<RouteController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(AdminViewPaths.ums + "Route.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}