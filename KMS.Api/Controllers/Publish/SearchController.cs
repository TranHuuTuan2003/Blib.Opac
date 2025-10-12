using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using KMS.Api.Services;
using KMS.Shared.DTOs.FacetFilter;
using KMS.Shared.DTOs.Search;
using KMS.Shared.Helpers;

using UC.Core.Models;

namespace KMS.Api.Controllers.Publish
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    // [EnableRateLimiting("Search")]
    public class SearchController : ControllerBase
    {
        private readonly IServiceWrapper _service;
        private readonly ILogger<SearchController> _logger;
        private readonly IConfiguration _configuration;

        public SearchController(
            IServiceWrapper service,
            ILogger<SearchController> logger,
            IConfiguration configuration
            )
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("{type}/{page}/{pageSize}")]
        public async Task<IActionResult> SearchingAsync([FromRoute] string type, [FromRoute] int page, [FromRoute] int pageSize, [FromBody] SearchBody searchRequest)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                var items = await _service.search_service.SearchingAsync(type, page, pageSize, searchRequest);
                return ResponseMessage.Success(items);
            });
        }

        [HttpPost("FacetFilterAsync/{type}")]
        public async Task<IActionResult> FacetFilterAsync(string type, [FromBody] FacetFilterRequest facetFilterRequest)
        {
            return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
            {
                var item = await _service.search_service.FacetFilterAsync(type, facetFilterRequest);
                return ResponseMessage.Success(item);
            });
        }
    }
}
