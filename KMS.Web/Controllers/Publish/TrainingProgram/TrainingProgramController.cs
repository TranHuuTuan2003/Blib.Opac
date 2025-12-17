using KMS.Shared.DTOs.Document;
using KMS.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;

namespace KMS.Web.Controllers.Publish.TrainingProgram
{
    public class TrainingProgramController : Controller
    {
        private readonly ILogger<TrainingProgramController> _logger;
        private readonly AppConfigHelper _appConfigHelper;
        private readonly ApiHelper _apiHelper;
        //private readonly IService _service;

        public TrainingProgramController(ILogger<TrainingProgramController> logger, AppConfigHelper appConfigHelper, ApiHelper apiHelper/*, IService service*/)
        {
            _logger = logger;
            _appConfigHelper = appConfigHelper;
            _apiHelper = apiHelper;
            //_service = service;
        }

        [Route("chuong-trinh-dao-tao")]
        public async Task<IActionResult> Index()
        {
            return View("~/Views/TrainingProgram/Index.cshtml");
        }
    }
}