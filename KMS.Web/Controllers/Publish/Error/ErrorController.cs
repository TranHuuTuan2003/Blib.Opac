using Microsoft.AspNetCore.Mvc;

namespace KMS.Web.Controllers.Publish.Error
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("khong-tim-thay-tai-nguyen-yeu-cau")]
        public IActionResult SourcesNotFound()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}