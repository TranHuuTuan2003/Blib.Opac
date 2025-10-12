using Microsoft.AspNetCore.Mvc;
using KMS.Web.Areas.Admin.Common;

namespace KMS.Web.Areas.Admin.Controllers.Auth
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        public IActionResult Error419()
        {
            return View(AdminViewPaths.error + "Error419.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}