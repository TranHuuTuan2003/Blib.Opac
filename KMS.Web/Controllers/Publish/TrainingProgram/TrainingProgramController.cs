using KMS.Shared.DTOs.Document;
using KMS.Shared.Helpers;
using KMS.Web.Helpers;
using KMS.Web.Services.TrainingProgram;
using KMS.Web.ViewModels.Shared.Components.SearchPage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using SmartBreadcrumbs.Attributes;
using System.Reflection;

namespace KMS.Web.Controllers.Publish.TrainingProgram
{
    public class TrainingProgramController : Controller
    {
        private readonly ILogger<TrainingProgramController> _logger;
        private readonly AppConfigHelper _appConfigHelper;
        private readonly ApiHelper _apiHelper;
        private readonly IService _service;

        public TrainingProgramController(ILogger<TrainingProgramController> logger, AppConfigHelper appConfigHelper, ApiHelper apiHelper, IService service)
        {
            _logger = logger;
            _appConfigHelper = appConfigHelper;
            _apiHelper = apiHelper;
            _service = service;
        }

        [Route("chuong-trinh-dao-tao/{id}")]
        public async Task<IActionResult> Index(string id)
        {
            var item = await _service.GetDetailAsync(id);
            return View("~/Views/TrainingProgram/Index.cshtml", item);
        }

        [HttpPost("chuong-trinh-dao-tao/{id}")]
        public async Task<IActionResult> Index2(string id)
        {
            try 
            { 
                var item = await _service.GetDetailAsync(id);
                return PartialView("TrainingProgram/_TrainingProgram", item);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(_logger, ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return PartialView("SearchPage/_SearchResults", new SearchResultViewModel());
            }
        }
    }
}