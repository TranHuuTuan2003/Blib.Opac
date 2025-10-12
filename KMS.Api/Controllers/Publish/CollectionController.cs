using KMS.Api.Filters;
using KMS.Api.Services;
using KMS.Shared.DTOs.Document;
using KMS.Shared.DTOs.Search;
using KMS.Shared.Helpers;
using Microsoft.AspNetCore.Mvc;
using UC.Core.Models;

namespace KMS.Api.Controllers.Publish
{
    [ApiController]
    [Route("api/[controller]")]
    public class CollectionController : ControllerBase
    {
        private readonly IServiceWrapper _service;
        private readonly ILogger<CollectionController> _logger;

        public CollectionController(
            IServiceWrapper service,
            ILogger<CollectionController> logger
            )
        {
            _service = service;
            _logger = logger;
        }

        //[HttpGet("GetCollectionByDbType")]
        //public async Task<IActionResult> GetCollections(string dbType)
        //{
        //    return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
        //    {
        //        var item = await _service.collection.CollectionTreeDbTypeAsync(dbType);
        //        return ResponseMessage.Success(item);
        //    });
        //}

        //[HttpPost("GetCollectionItems/{type}/{page}/{pageSize}")]
        //public async Task<IActionResult> SearchingAsync([FromRoute] string type, [FromRoute] int page, [FromRoute] int pageSize, [FromBody] SearchBody searchRequest)
        //{
        //    return await ControllerHelper.ExecuteWithHandlingAsync(_logger, async () =>
        //    {
        //        var items = await _service.collection.SearchingAsync(type, page, pageSize, searchRequest);
        //        return ResponseMessage.Success(items);
        //    });
        //}
    }
}