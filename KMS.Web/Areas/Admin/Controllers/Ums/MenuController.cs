using Microsoft.AspNetCore.Mvc;
using KMS.Web.Areas.Admin.Common;

namespace KMS.Web.Areas.Admin.Controllers.Ums
{
    [Area("Admin")]
    public class MenuController : Controller
    {
        private readonly ILogger<MenuController> _logger;

        public MenuController(ILogger<MenuController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(AdminViewPaths.ums + "Menu.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}