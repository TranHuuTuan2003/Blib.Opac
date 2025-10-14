using System.Reflection;

using Microsoft.AspNetCore.Mvc;

using KMS.Shared.DTOs.Search;
using KMS.Shared.Helpers;
using KMS.Web.Services.Search;
using KMS.Web.ViewModels.Shared.Components.SearchPage;
using KMS.Web.ViewModels.Shared.Components.DocumentDetail;

namespace KMS.Web.Controllers.Publish.Search
{
    public class SearchController : Controller
    {
        private readonly ILogger<SearchController> _logger;
        private readonly IService _service;

        public SearchController(ILogger<SearchController> logger, IService service)
        {
            _logger = logger;
            _service = service;
        }

        [Route("tim-kiem")]
        public IActionResult Index()
        {
            return View();
        }

		[Route("tim-kiem-tai-lieu-so")]
		public IActionResult Collection()
		{
			return View("~/Views/Search/CollectionDDoc.cshtml");
		}

        [Route("tim-kiem-tai-lieu-in")]
        public IActionResult CollectionPDoc()
        {
            return View("~/Views/Search/CollectionPDoc.cshtml");
        }

        [HttpPost("tim-kiem")]
        public async Task<IActionResult> Index([FromBody] SearchRequest searchRequest)
        {
            try
            {
                var searchResult = await _service.SearchDocsAsync(searchRequest);
                return PartialView("SearchPage/_SearchResults", searchResult);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(_logger, ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return PartialView("SearchPage/_SearchResults", new SearchResultViewModel());
            }
        }

        [HttpPost("tim-kiem-tai-lieu-so")]
        public async Task<IActionResult> CollectionDdoc([FromBody] SearchRequest searchRequest)
        {
            try
            {
                var searchResult = await _service.SearchDocsCollectionDdocAsync(searchRequest);
                return PartialView("SearchPage/_SearchResults", searchResult);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(_logger, ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return PartialView("SearchPage/_SearchResults", new SearchResultViewModel());
            }
        }

        [HttpPost("tim-kiem-tai-lieu-in")]
        public async Task<IActionResult> CollectionPdoc([FromBody] SearchRequest searchRequest)
        {
            try
            {
                var searchResult = await _service.SearchDocsCollectionPdocAsync(searchRequest);
                return PartialView("SearchPage/_SearchResults", searchResult);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(_logger, ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return PartialView("SearchPage/_SearchResults", new SearchResultViewModel());
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}
