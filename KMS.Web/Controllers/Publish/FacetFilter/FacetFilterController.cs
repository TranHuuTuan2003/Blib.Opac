using Microsoft.AspNetCore.Mvc;

using KMS.Shared.DTOs.FacetFilter;
using KMS.Shared.Helpers;
using KMS.Web.Services.FacetFilter;
using KMS.Web.ViewModels.Shared.Pages.SearchPage;

namespace KMS.Web.Controllers.Publish.FacetFilter
{
    public class FacetFilterController : Controller
    {
        private readonly ILogger<FacetFilterController> _logger;
        private readonly IService _service;

        public FacetFilterController(ILogger<FacetFilterController> logger, IService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost("FacetFilter/{type}")]
        public async Task<IActionResult> Index(string type, [FromBody] FacetFilterRequest facetFilterRequest)
        {
            try
            {
                var viewModel = await _service.GetFacetFilterAsync(type, facetFilterRequest);
                return PartialView("SearchPage/_FacetFilters", viewModel);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(_logger, ex, ex.Message);
                return PartialView("SearchPage/_FacetFilters", new FacetFilterViewModel());
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}